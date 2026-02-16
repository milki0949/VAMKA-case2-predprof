tableBody = document.getElementById("table-body");
productApplicationButton = document.getElementById("product-application-button");

async function subtractQuantityProduct(id, quantitySubtract) {
    try {
        api = "/api/product/subtract";
        const subtractProductInput = {
            id: id,
            quantityToSubtract: quantitySubtract
        }

        const response = await fetch(api, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(subtractProductInput)
        });

        renderDish();
    }
    catch (e) {
        console.error("Ошибка сохранения", e);
        alert("Не удалось сохранить статус!");
    }
}

async function renderDish() {
    try {
        api = '/api/products';

        const response = await fetch(api);

        if (!response.ok) throw new Error("Ошибка сервера");
        const products = await response.json(); 

        tableBody.innerHTML = '';

        products.forEach(product => {
            const row = document.createElement('tr');
            const inputId = `quantity-to-subtract-${product.id}`;

            row.innerHTML = `
                <td class="col-id">${product.id}</td>
                <td class="col-name">${product.productName}</td>
                <td class="col-quantity">${product.quantity}</td>
                <td class="col-input">
                <input id="${inputId}" class="quantity-to-subtract" type="number" placeholder="123"/> 
                <button id="subtract-button" type="submit" onclick="subtractQuantityProduct(${product.id}, Number(document.getElementById('${inputId}').value))">-</button></td>
            `;

            tableBody.appendChild(row);
        });

    }
    catch (e) {
        tableBody.innerHTML = `<tr><td colspan="5" style="color:red">Ошибка: ${e.message}</td></tr>`;
    }
}

productApplicationButton.addEventListener('click', function () {
    window.location.href = '/productApplication.html'
});

document.addEventListener('DOMContentLoaded', renderDish());