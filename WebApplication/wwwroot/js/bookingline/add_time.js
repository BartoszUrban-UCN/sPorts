function AddTimeButtonClicked() {
    $("#addtime-button").css("pointer-events", "none");
    $("#addtime-button").css("cursor", "default");
    $("#addtime-button").addClass("disabled");
    $("#addtime-button").find("span").addClass("spinner-border");
    $("#addtime-button").find("span").addClass("spinner-border-sm");
}

$(function () {
    $("#addtime-button").on("click", AddTimeButtonClicked);
})