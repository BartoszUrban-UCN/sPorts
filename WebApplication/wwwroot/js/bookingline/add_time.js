function AddTimeButtonClicked() {
    $("#addtime-button").css("pointer-events", "none");
    $("#addtime-button").css("cursor", "default");
    $("#addtime-button").addClass("disabled");
    $("#addtime-button").find("span").addClass("spinner-border");
    $("#addtime-button").find("span").addClass("spinner-border-sm");

    var bookingLineId = $(this).attr("data-bookingLine-id")

    $.ajax({
        type: "PUT",
        url: "/api/booking/" + bookingLineId + "/AddTime",
        data: { bookingLineId: bookingLineId },
        success: function (result) {
            //location.reload();
        },
        error: {}
    });
}

$(function () {
    $("#addtime-button").on("click", AddTimeButtonClicked);
})