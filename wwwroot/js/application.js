document.addEventListener('DOMContentLoaded', loadApplications);

async function loadApplications() {
    const container = document.getElementById('applications-container');
    if (!container) return;

    container.innerHTML = 'Загрузка заявок...';

    try {
        const response = await fetch('/api/product/application');
        if (!response.ok) throw new Error('Ошибка загрузки заявок');

        const applications = await response.json();
        container.innerHTML = ''; 

        applications.forEach(app => {
            const products = app.products || app.Products || [];
            const total = Number(app.totalAmount ?? app.totalPrice ?? app.TotalPrice ?? 0);
            const date = app.date || app.Date || '';
            const appId = app.id ?? app.Id;
            const status = app.status ?? app.Status;

            const productsHtml = products.map(p => {
                const name = p.productName || p.ProductName || 'Неизвестный';
                const qty = p.quantity || p.Quantity || 0;
                const priceNum = Number(p.price ?? p.Price ?? 0);
                const priceText = isFinite(priceNum) ? priceNum.toFixed(2) : '0.00';
                return `
                <div class="item">
                    <span>${name}</span>
                    <span>${qty} кг</span>
                    <span>${priceText} р.</span>
                </div>
            `;
            }).join('');

            const encodedProducts = encodeURIComponent(JSON.stringify(products));

            const card = `
                <div class="card" id="app-card-${appId}">
                    <div class="header">
                        Закупка на ${date} 
                        <span style="float:right; color:${status ? 'green' : 'red'};">${status ? 'Одобрено' : 'В ожидании'}</span>
                    </div>
                    <div class="content">
                        ${productsHtml}
                        <div class="total-row">
                            <div class="total-label">Итого:</div>
                            <div class="total-amount">${total.toFixed(2)} р.</div>
                        </div>
                    </div>
                    <div class="buttons">
                        <button class="btn btn-approve" ${status ? 'disabled' : ''} data-app-id="${appId}" data-products="${encodedProducts}">Одобрить</button>
                        <button class="btn btn-reject" ${status ? 'disabled' : ''} data-app-id="${appId}">Отклонить</button>
                    </div>
                </div>
            `;
            container.insertAdjacentHTML('beforeend', card);

            const insertedCard = document.getElementById(`app-card-${appId}`);
            if (insertedCard) {
                const btnApprove = insertedCard.querySelector('.btn-approve');
                const btnReject = insertedCard.querySelector('.btn-reject');

                if (btnApprove) {
                    btnApprove.addEventListener('click', () => {
                        const raw = btnApprove.dataset.products || '[]';
                        let parsed = [];
                        try {
                            parsed = JSON.parse(decodeURIComponent(raw));
                        } catch (e) {
                            console.error('Ошибка парсинга products:', e);
                        }
                        approveApplication(Number(btnApprove.dataset.appId), parsed, total);
                    });
                }

                if (btnReject) {
                    btnReject.addEventListener('click', () => {
                        const id = Number(btnReject.dataset.appId);
                        rejectApplication(id);
                    });
                }
            }
        });

    } catch (err) {
        container.innerHTML = `<p style="color:red;">Ошибка загрузки: ${err.message}</p>`;
        console.error("Ошибка загрузки заявок:", err);
    }
}

async function approveApplication(appId, products, totalSpent) {
    if (!confirm(`Вы уверены, что хотите одобрить заявку #${appId}?`)) return;

    try {
        const response = await fetch('/api/products/application/approve', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ applicationId: appId, products: products, totalSpent: totalSpent })
        });

        if (response.ok) {
            alert("Заявка одобрена!");
            const card = document.getElementById(`app-card-${appId}`);
            if (card) {
                const header = card.querySelector('.header');
                if (header) header.innerHTML += ' <span style="color:green; float:right;">Одобрено</span>';
                const btnApprove = card.querySelector('.btn-approve');
                const btnReject = card.querySelector('.btn-reject');
                if (btnApprove) btnApprove.disabled = true;
                if (btnReject) btnReject.disabled = true;
            }
        } else {
            const err = await response.json();
            alert("Ошибка одобрения: " + err.message);
        }
    } catch (e) {
        console.error(e);
        alert("Ошибка сети при одобрении.");
    }
}

async function rejectApplication(appId) {
    try {
        const response = await fetch(`/api/products/application/reject/${appId}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            alert("Заявка отклонена и удалена!");
            const card = document.getElementById(`app-card-${appId}`);
            if (card) card.remove();
        } else {
            const err = await response.json();
            alert("Ошибка отклонения: " + err.message);
        }
    } catch (e) {
        console.error(e);
        alert("Ошибка сети при отклонении.");
    }
}
