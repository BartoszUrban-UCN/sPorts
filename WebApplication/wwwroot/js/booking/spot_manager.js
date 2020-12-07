function error() {
    alert("Wtf maan")
}

function confirmSpotBooked() {
    $(this).prop('disabled', true);
    $(this).find("span").addClass("spinner-border");
    $(this).find("span").addClass("spinner-border-sm");
    var bookingLineId = $(this).attr("data-bookingLine-id")
    $.ajax({
        type: "PUT",
        url: "/api/booking/" + bookingLineId + "/bookinglineconfirmation",
        data: { bookingLineId: bookingLineId },
        success: function (result) {
            location.reload();
            //$("#marinaOwnerBookings").on('click', '.cancelBtn', function () {
            //    $(this).closest('tr').fadeOut(1000);
            //});
        },
        error: {}
    });
}

function cancelSpotBooked() {
    $(this).prop('disabled', true);
    $(this).find("span").addClass("spinner-border");
    $(this).find("span").addClass("spinner-border-sm");
    var bookingLineId = $(this).attr("data-bookingLine-id")
    $.ajax({
        type: "PUT",
        url: "/api/booking/" + bookingLineId + "/bookinglinecancellation",
        data: { bookingLineId: bookingLineId },
        success: function (result) {
            location.reload();
            //$("#marinaOwnerBookings").on('click', '.cancelBtn', function () {
            //    $(this).closest('tr').fadeOut(1000);
            //});
        },
        error: {}
    });
}

$(function () {
    $("button.confirmBtn").on("click", confirmSpotBooked)

    $("button.cancelBtn").on("click", cancelSpotBooked)
})