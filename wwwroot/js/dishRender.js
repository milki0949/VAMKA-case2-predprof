tableBody = document.getElementById("table-body");

async function addQuantityDish(id, addQuantity) {
    try {
        api = "/api/dish/add";
        const AddQuantityDishInput = {
            id: id,
            quantityToAdd: addQuantity
        }

        const response = await fetch(api, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(AddQuantityDishInput)
        });

        renderDishes();
    }
    catch (e) {
        console.error("Ошибка сохранения", e);
        alert("Не удалось сохранить статус!");
    }
}

async function subtractQuantityDish(id, subtractQuantity) {
    try {
        api = "/api/dish/subtract";
        const SubtractQuantityDishInput = {
            id: id,
            quantityToSubtract: subtractQuantity
        }

        const response = await fetch(api, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(SubtractQuantityDishInput)
        });

        renderDishes();
    }
    catch (e) {
        console.error("Ошибка сохранения", e);
        alert("Не удалось сохранить статус!");
    }
}

async function renderDishes() {
    try {
        api = '/api/dishes';

        const response = await fetch(api);

        if (!response.ok) throw new Error("Ошибка сервера");
        const dishes = await response.json();

        tableBody.innerHTML = '';

        dishes.forEach(dish => {
            const row = document.createElement('tr');
            const inputId = `add-to-quantity${dish.id}`;

            row.innerHTML = `
                <td class="col-id">${dish.id}</td>
                <td class="col-name">${dish.dishName}</td>
                <td class="col-quantity">${dish.quantity}</td>
                <td class="col-input">
                <button id="subtract-button" type="submit" onclick="subtractQuantityDish(${dish.id}, Number(document.getElementById('${inputId}').value))">-</button>
                <input id="${inputId}" class="quantity-to-subtract" type="number" placeholder="123"/> 
                <button id="add-button" type="submit" onclick="addQuantityDish(${dish.id}, Number(document.getElementById('${inputId}').value))">+</button></td>
            `;

            tableBody.appendChild(row);
        });

    }
    catch (e) {
        tableBody.innerHTML = `<tr><td colspan="5" style="color:red">Ошибка: ${e.message}</td></tr>`;
    }
}

document.addEventListener('DOMContentLoaded', renderDishes());