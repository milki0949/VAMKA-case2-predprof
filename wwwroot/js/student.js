const getBreakfastButton = document.getElementById('get-breakfast-button');
const getLunchButton = document.getElementById('get-lunch-button');
const payButton = document.getElementById('pay-button');
const menuButton = document.getElementById('menu-button');
const allergyButton = document.getElementById('allergy-button');
const dislikeProductButton = document.getElementById('dislike-product-button');
const dislikeDishButton = document.getElementById('dislike-dish-button');
const userJson = localStorage.getItem('user');
const user = JSON.parse(userJson);
const userId = user.id;

getBreakfastButton.addEventListener('click', async function () {
    const api = `api/student/get/breakfast/${userId}`;

    try {
        const response = await fetch(api, {
            method: 'POST'
        });

        const gotBreakfast = await response.json();

        if (response.ok) {
            if (gotBreakfast) {
                this.style.backgroundColor = '#4CAF50';
                this.textContent = 'завтрак получен'; 
                this.disabled = true;   
            }
        }
        else {
            console.error('Ошибка сервера:', response.status);
        }
    }
    catch (e) {
        console.error('Сетевая ошибка:', e);
    }
});

getLunchButton.addEventListener('click', async function () {
    const api = `api/student/get/lunch/${userId}`;

    try {
        const response = await fetch(api, {
            method: 'POST'
        });

        const gotLunch = await response.json();

        if (response.ok) {
            if (gotLunch) {
                this.style.backgroundColor = '#4CAF50';
                this.textContent = 'обед получен';
                this.disabled = true;
            }
        }
        else {
            console.error('Ошибка сервера:', response.status);
        }
    }
    catch (e) {
        console.error('Сетевая ошибка:', e);
    }
});

payButton.addEventListener('click', function () {
    window.location.href = '/pay.html';
});

menuButton.addEventListener('click', function () {
    window.location.href = '/menu.html';
});

allergyButton.addEventListener('click', function () {
    window.location.href = '/allergy.html'
});

dislikeProductButton.addEventListener('click', function () {
    window.location.href = '/dislikeProduct.html';
});

dislikeDishButton.addEventListener('click', function () {
    window.location.href = '/dislikeDish.html';
});