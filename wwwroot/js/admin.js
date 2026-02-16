const applicationButton = document.getElementById("application-button");
const nutritionReportButton = document.getElementById("nutrition-report-button");
const costReportButton = document.getElementById("cost_report-button");

applicationButton.addEventListener("click", function () {
    window.location.href = "/application.html";
});

nutritionReportButton.addEventListener("click", function () {
    window.location.href = "/nutritionReport.html";
});

costReportButton.addEventListener("click", function () {
    window.location.href = "/costReport.html";
});