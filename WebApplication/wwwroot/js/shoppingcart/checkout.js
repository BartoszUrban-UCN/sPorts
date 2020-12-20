function checkInvalid(data, status) {
    alert(data)
}

function CheckoutButtonClicked() {
    $("#checkout-button").css("pointer-events", "none");
    $("#checkout-button").css("cursor", "default");
    $("#checkout-button").addClass("disabled");
    $("#checkout-button").find("span").addClass("spinner-border");
    $("#checkout-button").find("span").addClass("spinner-border-sm");
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

$(function () {
    // Create an instance of the Stripe object with the publishable API key
    var stripe = Stripe("pk_test_51HwQzXFX3jJVK0RyYBNAbadJnDpDzb4VQ2WPIJN8b5jpIPC20SIhXVNj0d6IsJdLpf8eXYrm1hg9EKcIfrxrCFe50070XWr5Zg");

    $("#checkout-button").on("click", function () {
        CheckoutButtonClicked();

        fetch("/create-payement-session", {
            method: "POST",
        })
        .then(function (response) {
            return response.json();
        })
        .then(function (session) {
            return stripe.redirectToCheckout({ sessionId: session.id });
        })
        .then(function (result) {
            // If redirectToCheckout fails due to a browser or network
            // error, you should display the localized error message to your
            // customer using error.message.
            if (result.error) {
                alert(result.error.message);
            }
        })
        .catch(function (error) {
            console.error("Error:", error);
        });
    });

    $("input.simulateBtn").on("click", CreateBookingFromSession);
})