// A $( document ).ready() block.
$(document).ready(function () {


    // get and show the first index search
    var firstSelectedIndexId = $("#search-select-index").val();
    console.log("firstSelectedIndexId: " + firstSelectedIndexId);
    onSearchSelectIndexChange(firstSelectedIndexId);

    // binding select event
    $('#search-select-index').change(function () {
        onSearchSelectIndexChange($(this).val());
    });



    $('#search-form').submit(function () {
        $(".loading-overlay").show();
        setValueSearchToForm();

        // get current value for
        return true;
    });
});

function onSearchSelectIndexChange(id) {

    // disable current show input
    $(".index-select-input").hide();

    // show new index input
    $("#search-index-" + id).show();
}

function onAddSearchConditionClick() {
    setValueSearchToForm();

    // get selected value
    var id = getSelectIndexTitleId();

    // show hidden field
    $("#row-search-input-group-" + id).show();
}

function onRemoveSearchInput(id) {

    $("#row-search-input-group-" + id).hide();

    // remove value
    $("#search-input-index-" + id).val("");
}

function getSelectIndexTitleId() {
    return $("#search-select-index").val();
}

function getSelectIndexTitleValue(id) {
    return $("#search-index-" + id).val();
}

function setValueSearchToForm() {
    var indexSelectId = getSelectIndexTitleId();
    var indexSelectValue = getSelectIndexTitleValue(indexSelectId);

    // if have value in input search, add this value to form input
    if (indexSelectValue) {
        $("#search-input-index-" + indexSelectId).val(indexSelectValue);
    }
}

function UpdateStatusSearchIndex() {

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
        id = $("#cabinet-id").val();
    });
    var args = {
        CustomerStatus: data,
        CabinetId: id
    };
    if (data != "") {
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