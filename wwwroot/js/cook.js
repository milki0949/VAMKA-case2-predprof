productApplicationButton = document.getElementById('product-application-button');
productControleButton = document.getElementById('products-controle-button');
breakfastDistributionButton = document.getElementById('breakfast-distribution');
lunchDistributionButton = document.getElementById('lunch-distribution');
dishControleButton = document.getElementById('dish-controle-button');

dishControleButton.addEventListener('click', function () {
    window.location.href = '/dishControle.html';
});

lunchDistributionButton.addEventListener('click', function () {
    window.location.href = '/lunchDistribution.html';
});

breakfastDistributionButton.addEventListener('click', function () {
    window.location.href = '/breakfastDistribution.html';
});

productApplicationButton.addEventListener('click', function () {
    window.location.href = '/productApplication.html';
});

productControleButton.addEventListener('click', function () {
    window.location.href = '/productControle.html';
});