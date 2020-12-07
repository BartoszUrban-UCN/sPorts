function checkInvalid(data, status) {
    alert(data)
}

function checkoutButtonClicked() {
    //$(this).prop('disabled', true);
    $(this).css("pointer-events", "none");
    $(this).css("cursor", "default");
    $(this).addClass("disabled");
    $(this).find("span").addClass("spinner-border");
    $(this).find("span").addClass("spinner-border-sm");

    //var url = $(this).attr("name")
    //$.ajax({
    //    type: "POST",
    //    url: url,
    //    success: () => { },
    //    error: error
    //})
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