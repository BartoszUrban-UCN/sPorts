function checkInvalid(data, status) {
    alert(data)
}

function checkoutButtonClicked() {
    $.get("Checkout",
        (data, status) => checkInvalid(data, status))
}

$(function() {
    $("a.checkoutBtn").on("click", checkoutButtonClicked)
})