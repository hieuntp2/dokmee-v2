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
});

function onSearchSelectIndexChange(id) {

  // disable current show input
  $("input[name=index-select-input]").hide();

  // show new index input
  $("#search-index-" + id).show();
}