$(document).ready(function () {
    console.log("ready!");
    $('input').iCheck({
        checkboxClass: 'icheckbox_minimal-blue',
        radioClass: 'iradio_minimal-blue',
        increaseArea: '20%' // optional
    });

    $('#checkbox-all').on('ifChanged', function (event) {
        if ($('#checkbox-all').prop('checked')) {
            $(".select-doc-item").iCheck('check');
        } else {
            $(".select-doc-item").iCheck('uncheck');
        }
    });
});

function preview(id) {
    if (!id) {
        return;
    }
    $(".loading-overlay").show();
    var url = $("#preview").val() + '?id=' + id;
    console.log(url);
    $.ajax({
        url: url,
        type: 'post',
        contentType: "application/json; charset=utf-8",
        success: function (rs) {
            $(".loading-overlay").hide();
            if (!rs.isError) {
                console.log(rs.url);
                var win = window.open(rs.url, '_blank');
                if (win) {
                    //Browser has allowed it to be opened
                    win.focus();
                } else {
                    //Browser has blocked it
                    swal({
                        type: 'error',
                        title: 'Oops...',
                        text: 'Please allow popups for this website!'
                    });
                }
            } else {
                var message = "Something wrong. Please try again.";
                if (rs.message) message = rs.message;
                swal({
                    type: 'error',
                    title: 'Oops...',
                    text: message
                });
            }
        },
        error: function (rs) {
            $(".loading-overlay").hide();
            swal({
                type: 'error',
                title: 'Oops...',
                text: 'Something wrong. Please try again.'
            });
        }
    });
}

function complete() {

    var trIdx = [];
    var data = "";
    var idx = 1;
    $('#tblfileSystems > tbody  > tr').each(function () {
        var select = $(this).find('input[type="checkbox"]:checked');
        if (select.is(":checked")) {
            var nodeId = $(this).attr('nodeId');
            if (data != "") {
                data = data + ";";
            }
            data = data + nodeId;
            trIdx.push(idx);
        }
        idx = idx + 1;
    });

    if (!data) {
        swal({
            text: 'Please select document!'
        });
        return;
    } else {

        var args = {
            NodeId: data
        };
        swal({
            title: 'Are you sure?',
            text: "Files seleted will be move to another folder and change status to Complete",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes'
        }).then((result) => {
            if (!result.value) {
                return;
            }

            if (data != "") {
                $(".loading-overlay").show();
                var url = $("#complete").val();;
                $.ajax({
                    url: url,//'/home/Complete',
                    type: 'post',
                    data: '{"args":' + JSON.stringify(args) + '}',
                    contentType: "application/json; charset=utf-8",
                    success: function (rs) {
                        for (var i = trIdx.length - 1; i >= 0; i--) {
                            var rowdelete = trIdx[i];
                            console.log(rowdelete);
                            document.getElementById('tblfileSystems').deleteRow(rowdelete);
                        }
                        $(".loading-overlay").hide();
                    },
                    error: function (rs) {
                        var message = "Something wrong. Please try again.";
                        if (rs.message) message = rs.message;
                        swal({
                            type: 'error',
                            title: 'Oops...',
                            text: message
                        });
                        $(".loading-overlay").hide();
                    }
                });
            }
        });
    }

}

function updateAllStatus() {
    var status = $("#update-all-status-value").val();

    if (!status) {
        swal({
            text: 'Please select status!'
        });
        return;
    }

    var args = getSelectedDocument(status);
    if (!args.CustomerStatus) {
        swal({
            text: 'Please select document!'
        });
        return;
    }

    $(".loading-overlay").show();
    var url = $("#updatestatus").val();;
    $.ajax({
        url: url,
        type: 'post',
        data: '{"args":' + JSON.stringify(args) + '}',
        contentType: "application/json; charset=utf-8",
        success: function (rs) {
            setViewSelectedDocumentStatus(status);
            swal({
                type: 'success',
                text: 'Update sucessfully!'
            });
            $(".loading-overlay").hide();
        },
        error: function (rs) {
            var message = "Something wrong. Please try again.";
            if (rs.message) message = rs.message;
            swal({
                type: 'error',
                title: 'Oops...',
                text: message
            });
            $(".loading-overlay").hide();
        }
    });
}

function UpdateStatusSearchIndex() {
    var args = getSelectedDocument();
    if (!args.CustomerStatus) {
        swal({
            text: 'Please select document!'
        });
        return;
    }

    var isNotSelectedStatus = true;
    $('#tblfileSystems > tbody  > tr').each(function () {
        var select = $(this).find('input[type="checkbox"]:checked');
        if (select.is(":checked")) {
            var status = $(this).find('#status').val();
            if (status) {
                isNotSelectedStatus = false;
            }
        }
    });
    if (isNotSelectedStatus) {
        swal({
            text: 'Please select document status!'
        });
        return;
    }
    var url = $("#updatestatus").val();;
    $(".loading-overlay").show();
    $.ajax({
        url: url,//'/home/UpdateStatus',
        type: 'post',
        data: '{"args":' + JSON.stringify(args) + '}',
        contentType: "application/json; charset=utf-8",
        success: function (rs) {
            swal({
                type: 'success',
                text: 'Update sucessfully!'
            });
            $(".loading-overlay").hide();
        },
        error: function (rs) {
            var message = "Something wrong. Please try again.";
            if (rs.message) message = rs.message;
            swal({
                type: 'error',
                title: 'Oops...',
                text: message
            });
            $(".loading-overlay").hide();
        }
    });
}

function getSelectedDocument(statusAll) {
    var data = "";
    $('#tblfileSystems > tbody  > tr').each(function () {
        var select = $(this).find('input[type="checkbox"]:checked');
        if (select.is(":checked")) {
            var nodeId = $(this).attr('nodeId');

            var status = $(this).find('#status').val();
            if (statusAll) status = statusAll; // if input status, then select input status
            if (data != "") {
                data = data + ";";
            }
            data = data + nodeId + ":" + status;
        }
    });
    return {
        CustomerStatus: data
    };
}

function setViewSelectedDocumentStatus(status) {
    $('#tblfileSystems > tbody  > tr').each(function () {
        var select = $(this).find('input[type="checkbox"]:checked');
        if (select.is(":checked")) {
            var nodeId = $(this).attr('nodeId');
            $(this).find('#status').selectpicker('val', status);
        }
    });
}


