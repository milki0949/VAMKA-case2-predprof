const loginInput = document.getElementById('login');
const passwordInput = document.getElementById('password');
const roleSelect = document.getElementById('role');
const loginButton = document.getElementById('login-button');

async function check(login, password) {
    api = '/api/check';
    const userInput = {
        login: login,
        userPassword: password
    };

    try {
        await fetch(api, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(userInput)
        });
    }
    catch (e) {
        console.error(e);
        //alert("Ошибка сети");
    }
}

async function login(role, login, password) {
    api = `/api/${role}`;
    const userInput = {
        login: login,
        userPassword: password
    };

    try {
        check(login, password);
        const response = await fetch(api, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(userInput)
        });

        if (response.ok) {
            const user = await response.json();
            localStorage.setItem('user', JSON.stringify(user));
            if (role === 'student') {
                window.location.href = '/student.html';
            }
            else if (role === 'cook') {
                window.location.href = '/cook.html';
            }
            else if (role === 'administrator') {
                window.location.href = '/admin.html';
            }
        }
        else {
            const error = await response.json();
            //alert("Ошибка: " + error.message);
        }
    }
    catch (e) {
        console.error(e);
        //alert("Ошибка сети");
    }
}

loginButton.addEventListener('click', async function () {
    const role = roleSelect.value;
    const userlogin = loginInput.value;
    const password = passwordInput.value;

    await login(role, userlogin, password);
});