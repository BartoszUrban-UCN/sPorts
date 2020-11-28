function error() {
    alert("Wtf maan")
}

function removeButtonClicked() {
    var bookingLineId = $(this).attr("name")
    $.ajax({
        type: "Delete",
        url: "/api/booking/RemoveBookingLine",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(bookingLineId),
        success: () => location.reload(),
        error: error
    })
}

function removeAllButtonClicked() {
    var bookingLineIds = $(this).attr("name")
}

$(function() {
    $("a.removeBtn").on("click", removeButtonClicked)

    $("a.removeAllBtn").on("click", removeAllButtonClicked)
})