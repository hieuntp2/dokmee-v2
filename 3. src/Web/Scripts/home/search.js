// A $( document ).ready() block.
$(document).ready(function () {

    // get and show the first index search
    var firstSelectedIndexId = $("#search-select-index").val();
    onSearchSelectIndexChange(firstSelectedIndexId, false);

    // binding select event
    $('#search-select-index').change(function () {
        onSearchSelectIndexChange($(this).val(), true);
    });



    $('#search-form').submit(function () {

        setValueSearchToForm();

        // validate input
        var haveInputSearch = false;
        $(".index-select-input").each(function () {
            if ($(this).val()) {
                haveInputSearch = true;
                return;
            }
        });

        if (!haveInputSearch) {
            swal({
                type: 'warning',
                title: 'Oops...',
                text: "Please select asleast search value"
            });
            return false;
        }

        $(".loading-overlay").show();
        // get current value for
        return true;
    });
});

function onSearchSelectIndexChange(id, clearValue) {
    // clear current search value
    if (clearValue) {
        $(".index-select-input").selectpicker('val', '');
        $(".index-select-input").val('');
    }
    
    // disable current show input
    // $(".index-select-input").hide();

    // show new index input
    //$("#search-index-" + id).show();
    $(".index-select-input-div").hide();
    $("#div-search-input-" + id).show();
}

function onAddSearchConditionClick() {
    if (!setValueSearchToForm()) {
        return;
    }

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
        $("#search-input-index-" + indexSelectId).selectpicker('val', indexSelectValue);
        return true;
    }
    return false;
}
