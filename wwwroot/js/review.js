const productSelect = document.getElementById('dish');
const reviewTextarea = document.getElementById('review-text');
const sendButton = document.getElementById('send-button');

async function renderDish() {
    const api = '/api/dishes/student';
    const response = await fetch(api);

    try {
        if (!response.ok) {
            throw new Error('Ошибка загрузки продуктов');
        }

        const dishes = await response.json();

        dishes.forEach(function (dish) {
            const option = document.createElement('option');

            option.value = dish.id;
            option.textContent = dish.dishName;

            productSelect.appendChild(option);
        });
    }
    catch (err) {
        console.error("Не удалось загрузить продукты:", err);
        const errorOption = document.createElement('option');
        errorOption.textContent = "Ошибка загрузки продуктов";
        productSelect.appendChild(errorOption);
    }
}

async function sendReview() {
    const api = '/api/dish/review';
    const reviewData = {
        dishId: Number(productSelect.value),
        reviewText: reviewTextarea.value
    }

    try {
        await fetch(api, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(reviewData)
        });
    }
    catch (e) {
        console.error("Ошибка сохранения", e);
    }
}

sendButton.addEventListener('click', function () {
    sendReview();
});

document.addEventListener('DOMContentLoaded', function () {
    renderDish();
});