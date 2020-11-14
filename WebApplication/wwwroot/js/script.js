$('.card').click(function(event) {
    var id = $(event.target).prop("card-text").text();
    window.open(`/marina/${id}/spots`)
})