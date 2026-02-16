const addSelectButton = document.getElementById('add-select-button');
const selectsContainer = document.getElementById('selects-container');
const rowTemplate = selectsContainer.querySelector('.select-row');
const sendButton = document.getElementById('send-button');
const productSelect = document.getElementById('products');
const userJson = localStorage.getItem('user');
const user = JSON.parse(userJson);
const userId = user.id;

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

document.addEventListener('DOMContentLoaded', function () {
    renderDish();
});

addSelectButton.addEventListener('click', function () {
    const newRow = rowTemplate.cloneNode(true);
    selectsContainer.appendChild(newRow);
});

sendButton.addEventListener('click', function () {
    sendApplication();
});

async function sendApplication() {
    const api = `/api/student/dislike/dish/${userId}`;

    dataToSend = [];

    const rows = selectsContainer.querySelectorAll('.select-row');
    rows.forEach(function (row) {
        const select = row.querySelector('select');

        const productValue = select.value;

        const product = {
            productId: Number(productValue)
        };

        dataToSend.push(product);
    });

    try {
        const response = await fetch(api, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dataToSend)
        });

        if (response.ok) {
            const user = await response.json();
            localStorage.setItem('user', JSON.stringify(user));
            console.log("Успех!");
        } else {
            console.error("Ошибка сервера:", response.status);
        }
    } catch (e) {
        console.error("Ошибка сети:", e);
    }
}