const addSelectButton = document.getElementById('add-select-button');
const selectsContainer = document.getElementById('selects-container');
const rowTemplate = selectsContainer.querySelector('.select-row');
const sendButton = document.getElementById('send-button');
const productSelect = document.getElementById('products');

async function renderDish() {
    const api = 'api/products/render';
   const response = await fetch(api);

   try {
       if (!response.ok) {
           throw new Error('Ошибка загрузки продуктов');
       }

       const products = await response.json();

       products.forEach(function (product) {
           const option = document.createElement('option');

           option.value = product.id;
           option.textContent = product.productName;

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
    newRow.querySelector('.quantiti').value = '';
    selectsContainer.appendChild(newRow);
});

sendButton.addEventListener('click', function () {
    sendApplication();
});

async function sendApplication() {
    const api = '/api/product/create/application';

    dataToSend = [];

    const rows = selectsContainer.querySelectorAll('.select-row');
    rows.forEach(function (row) {
        const select = row.querySelector('select');
        const quanititi = row.querySelector('.quantiti');

        const productValue = select.value;

        const product = {
            Id: Number(productValue),
            quantity: Number(quanititi.value)
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
            console.log("Успех!");
        } else {
            console.error("Ошибка сервера:", response.status);
        }
    } catch (e) {
        console.error("Ошибка сети:", e);
    }
}
