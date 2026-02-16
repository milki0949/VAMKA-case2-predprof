const reviewButton = document.getElementById('review-button');

function parseIds(str) {
    return str ? String(str).split(' ').map(s => Number(s)) : [];
}

async function renderMenu(apiUrl, columnSuffix, userData) {
    try {
        const response = await fetch(apiUrl);
        const weekMenu = await response.json();

        weekMenu.forEach(dayObj => {
            const dayName = dayObj.day;
            const dishes = dayObj.dishes;
            const column = document.getElementById(dayName + columnSuffix);

            if (!column) return;

            column.innerHTML = `<h3>${column.querySelector('h3').textContent}</h3>`;

            dishes.forEach(dish => {
                let cardStyle = '';
                let warningText = '';

                if (userData) {
                    const isDangerous = dish.productIds.some(id => userData.allergies.includes(id));

                    const hasDislikedIngredient = dish.productIds.some(id => userData.dislikeProducts.includes(id));
                    const isDislikedDish = userData.dislikeDishes.includes(dish.id);
                    const isDisliked = hasDislikedIngredient || isDislikedDish;

                    if (isDangerous) {
                        warningText = '<p style="color:red; font-weight:bold;">АЛЛЕРГИЯ!</p>';
                        cardStyle = 'style="border: 2px solid red; background-color: #ffe6e6;"';
                    }
                    else if (isDisliked) {
                        warningText = '<p style="color:orange; font-weight:bold;">Не нравится</p>';
                        cardStyle = 'style="border: 2px solid orange; background-color: #fff8e1;"';
                    }
                }

                const ingredientsHtml = dish.productNames
                    .map(name => `<p class="ingredient">${name}</p>`)
                    .join('');

                const dishCard = `
                    <div class="dish-block" ${cardStyle}>
                        <p class="dish-name">${dish.name}</p>
                        ${ingredientsHtml}
                        <p class="calories">${dish.calories} ккал</p>
                        ${warningText}
                    </div>
                `;

                column.insertAdjacentHTML('beforeend', dishCard);
            });
        });
    } catch (err) {
        console.error("Ошибка при загрузке меню:", err);
    }
}

document.addEventListener('DOMContentLoaded', async () => {
    const userJson = localStorage.getItem('user');
    let userData = null;

    if (userJson) {
        const user = JSON.parse(userJson);
        userData = {
            allergies: parseIds(user.allergy),
            dislikeProducts: parseIds(user.dislikeProducts),
            dislikeDishes: parseIds(user.dislikeDishes)
        };
    }

    await renderMenu('/api/menu/breakfast', '-column-breakfast', userData);
    await renderMenu('/api/menu/lunch', '-column-lunch', userData);
});

reviewButton.addEventListener('click', function () {
    window.location.href = '/review.html';
});
