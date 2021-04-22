$(function () {
    $("#stats-container").hide();
    $("#track-container").hide();

    $("#stats-btn").on("click", function () {
        $("#stats-container").slideToggle();
    });

    $("#track-btn").on("click", function () {
        $("#track-container").slideToggle();
    });
});