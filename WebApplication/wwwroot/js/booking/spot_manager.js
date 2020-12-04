function error() {
    alert("Wtf maan")
}

function confirmSpotBooked() {
    var bookingLineId = $(this).attr("data-bookingLine-id")
    $.ajax({
        type: "PUT",
        url: "/api/booking/" + bookingLineId + "/bookinglineconfirmation",
        data: { bookingLineId: bookingLineId },
        success: function (result) {
            $("#marinaOwnerBookings").on('click', '.confirmBtn', function () {
                $(this).closest('tr').fadeOut(1000);
            });
        },
        error: {}
    });
}

function cancelSpotBooked(ele) {
    var bookingLineId = $(this).attr("data-bookingLine-id")
    $.ajax({
        type: "PUT",
        url: "/api/booking/" + bookingLineId + "/bookinglinecancellation",
        data: { bookingLineId: bookingLineId },
        success: function (result) {
            $("#marinaOwnerBookings").on('click', '.cancelBtn', function () {
                $(this).closest('tr').fadeOut(1000);
            });
        },
        error: {}
    });
}

$(function () {
    $("input.confirmBtn").on("click", confirmSpotBooked)

    $("input.cancelBtn").on("click", cancelSpotBooked)
})