function checkInvalid(data, status) {
    alert(data)
}

function checkoutButtonClicked() {
    $(this).find("span").addClass("spinner-border");
    $(this).find("span").addClass("spinner-border-sm");
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