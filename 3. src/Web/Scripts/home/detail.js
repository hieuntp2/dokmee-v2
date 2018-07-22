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

function Check_Changed(element) {
    $('#tblfileSystems > tbody  > tr').find(':checkbox').prop('checked', element.checked);
}

function UpdateIndex() {
    $(".loading-overlay").show();
    var data = "";
    var id = "";
    $('#tblfileSystems > tbody  > tr').each(function () {
        var select = $(this).find('input[type="checkbox"]:checked');
        if (select.is(":checked")) {
            var nodeId = $(this).attr('nodeId');
            var status = $(this).find('#status').val()
            if (data != "") {
                data = data + ";";
            }
            data = data + nodeId + ":" + status;
        }
        id = $(this).attr('id');
    });
    var args = {
        CustomerStatus: data,
        CabinetId: id
    };
    if (data != "") {
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

function preview(cabinetId, id) {
    $.ajax({
        url: '/home/preview?id=' + id + '&&cabinetId=' + cabinetId,
        type: 'post',
        contentType: "application/json; charset=utf-8",
        success: function (rs) {
        },
        error: function (rs) {
        }
    });
}