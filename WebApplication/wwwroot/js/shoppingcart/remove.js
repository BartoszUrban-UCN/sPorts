// $(document).ready(function() {
//     $("input.removeBtn").click(function() {
//         var bookingLine = $(this).attr("data-bookingLine")
//         $.ajax({
//             type: "GET",
//             url: "/api/booking/removebookingline",
//             data: { bookingLine: bookingLine },
//             success: function(result) {
//                 console.log(bookingLine);
//                 $(".cartBookingLine").on('click', '.removeBtn', function() {
//                     $(this).closest('.cartBookingLine').fadeOut(1000);
//                 });
//             },
//             error: function(req, status, error) {
//                 alert("Something went wrong.")

//             }
//         });
//     })
// })

function success(bookingLineId) {
    alert("It Worked for id bookingLineId!")
}

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
        success: success(bookingLineId),
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