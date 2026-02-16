const payWeeklySubscriptionButton = document.getElementById('pay-weekly-subscription');
const payDaySubscriptionButton = document.getElementById('pay-day-subscription');
const userJson = localStorage.getItem('user');
const user = JSON.parse(userJson);
const userId = user.id;

async function payWeeklySubscription(studentId) {
    const api = `/api/subscription/weekly/${studentId}`;
    try {
        const response = await fetch(api, {
            method: 'POST'
        });
    }
    catch (e) {
        console.error(e);
        alert("Ошибка сети");
    }
}

async function payDaySubscription(studentId) {
    const api = `/api/subscription/day/${studentId}`;
    try {
        const response = await fetch(api, {
            method: 'POST'
        });
    }
    catch (e) {
        console.error(e);
        alert("Ошибка сети");
    }
}

payWeeklySubscriptionButton.addEventListener('click', function () {
    payWeeklySubscription(userId);
    alert('абонемент куплен')
});

payDaySubscriptionButton.addEventListener('click', function () {
    payDaySubscription(userId);
    alert('еда заказана')
});