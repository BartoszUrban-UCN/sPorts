function checkInvalid(data, status) {
    alert(data)
}

function checkoutButtonClicked() {
    var cartHasChanged = $(this).attr("name")
    if (cartHasChanged == 1) {
        $(".cartAlert").show();
    }
    //$.get("Checkout",
    //    (data, status) => checkInvalid(data, status))
}

function CreateBookingFromSession() {
    $.ajax({
        type: "POST",
        url: "/api/booking/CreateSampleBooking",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: () => { },
        error: error
    })
}

$(function() {
    $("a.checkoutBtn").on("click", checkoutButtonClicked)

    $("input.simulateBtn").on("click", CreateBookingFromSession)
})