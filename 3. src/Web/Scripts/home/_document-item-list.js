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
    $.ajax({
        url: '/home/preview?id=' + id,
        type: 'post',
        contentType: "application/json; charset=utf-8",
        success: function (rs) {
            $(".loading-overlay").hide();
            if (!rs.isError) {
                var win = window.open(rs.url, '_blank');
                if (win) {
                    //Browser has allowed it to be opened
                    win.focus();
                } else {
                    //Browser has blocked it
                    alert('Please allow popups for this website');
                }
            } else {
                alert("Something wrong. Please try again.");
            }
        },
        error: function (rs) {
            $(".loading-overlay").hide();
            alert("Something wrong. Please try again.");
        }
    });
}

function complete() {
    var trIdx = [];
    var data = "";
    var id = "";
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
        id = $(this).attr('id');
        idx = idx + 1;
    });
    var args = {
        NodeId: data,
        CabinetId: id
    };
    if (data != "") {
        $(".loading-overlay").show();
        $.ajax({
            url: '/home/Complete',
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
                alert("Complete fail!");
                $(".loading-overlay").hide();
            }
        });
    }
}

function updateAllStatus() {
    var status = $("#update-all-status-value").val();

    if (!status) {
        alert("Please select status!");
    }
}

function UpdateStatusSearchIndex() {

    var args = getSelectedDocument();
    if (args) {
        $(".loading-overlay").show();
        $.ajax({
            url: '/home/UpdateStatus',
            type: 'post',
            data: '{"args":' + JSON.stringify(args) + '}',
            contentType: "application/json; charset=utf-8",
            success: function (rs) {
                alert("Update sucessfully!");
                $(".loading-overlay").hide();
            },
            error: function (rs) {
                alert("Update fail!");
                $(".loading-overlay").hide();
            }
        });
    }
}

function getSelectedDocument() {
    var data = "";
    $('#tblfileSystems > tbody  > tr').each(function () {
        var select = $(this).find('input[type="checkbox"]:checked');
        if (select.is(":checked")) {
            var nodeId = $(this).attr('nodeId');
            var status = $(this).find('#status').val();
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

//function Check_Changed(element) {
//    $('#tblfileSystems > tbody  > tr').find(':checkbox').prop('checked', element.checked);
//}

//function UpdateIndex() {
//    var data = "";
//    var id = "";
//    $('#tblfileSystems > tbody  > tr').each(function () {
//        var select = $(this).find('input[type="checkbox"]:checked');
//        if (select.is(":checked")) {
//            var nodeId = $(this).attr('nodeId');
//            var status = $(this).find('#status').val()
//            if (data != "") {
//                data = data + ";";
//            }
//            data = data + nodeId + ":" + status;
//        }
//        id = $(this).attr('id');
//    });
//    var args = {
//        CustomerStatus: data,
//        CabinetId: id
//    };
//    if (data != "") {
//        $(".loading-overlay").show();
//        $.ajax({
//            url: '/home/UpdateStatus',
//            type: 'post',
//            data: '{"args":' + JSON.stringify(args) + '}',
//            contentType: "application/json; charset=utf-8",
//            success: function (rs) {
//                alert("Update sucessfully!");
//                $(".loading-overlay").hide();
//            },
//            error: function (rs) {
//                alert("Update fail!");
//                $(".loading-overlay").hide();
//            }
//        });
//    }
//}



