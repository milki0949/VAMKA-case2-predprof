const fullNameInput = document.getElementById('full-name');
const loginInput = document.getElementById('login');
const passwordInput = document.getElementById('password');
const repeatPasswordInput = document.getElementById('repeat-password');
const roleSelect = document.getElementById('role-select');
const codeInput = document.getElementById('code');
const registrationButton = document.getElementById('registration-button');

async function schoolCodeCheck(role, code) {
    const api = `/api/${role}/code`; 

    try {
        const response = await fetch(api); 
        if (!response.ok) return false;

        const data = await response.json();
        const corectCode = data.code;

        if (String(code) === String(corectCode)) { 
            return true;
        }
        else {
            return false;
        }
    } catch (e) {
        console.error(e);
        return false;
    }
}

function passwordCheckForRepetition(password, repeatPassword) {
    if (password === repeatPassword) {
        return true;
    }
    else {
        return false;
    }
}

async function isLoginExists(role, login) {
    const api = `/api/${role}/login/${login}`;

    try {
        const response = await fetch(api);
        const data = await response.json();

        if (data.isTaken) { 
            return false; 
        }
        else {
            return true;
        }
    } catch (e) {
        console.error(e);
        return false; 
    }
}

async function fullCheck(role, code, id, password, repeatPassword, login) {
    if (!passwordCheckForRepetition(password, repeatPassword)) {
        alert("Пароли не совпадают!");
        return false;
    }

    const loginTaken = await isLoginExists(role, login);
    if (loginTaken == false) {
        alert("Такой логин уже занят!");
        return false;
    }

    const codeValid = await schoolCodeCheck(role, code);
    if (!codeValid) {
        alert("Неверный код школы!");
        return false;
    }

    return true; 
}

async function registrationStudent(fullName, login, password) {
    const api = '/api/student/registration';
    const student = {
        fullName: fullName,
        login: login,
        userPassword: password,
    };
    try {
        const response = await fetch(api, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(student)
        });

        if (response.ok) {
            alert("Регистрация прошла успешно!");
            window.location.href = "/login.html";
        }
        else {
            const error = await response.json();
            alert("Ошибка: " + error.message);
        }
    }
    catch (e) {
        console.error(e);
        alert("Ошибка сети");
    }
}

async function registrationAdministrator(fullName, login, password) {
    const api = '/api/administrator/registration';
    const administrator = {
        fullName: fullName,
        login: login,
        userPassword: password
    };

    try {
        const response = await fetch(api, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(administrator)
        });

        if (response.ok) {
            alert("Регистрация прошла успешно!");
            window.location.href = "/login.html";
        }
        else {
            const error = await response.json();
            alert("Ошибка: " + error.message);
        }
    }
    catch (e) {
        console.error(e);
        alert("Ошибка сети");
    }
}

async function registrationCook(fullName, login, password) {
    const api = '/api/cook/registration';
    const cook = {
        fullName: fullName,
        login: login,
        userPassword: password
    };

    try {
        const response = await fetch(api, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(cook)
        });

        if (response.ok) {
            alert("Регистрация прошла успешно!");
            window.location.href = "/login.html";
        }
        else {
            const error = await response.json();
            alert("Ошибка: " + error.message);
        }
    }
    catch (e) {
        console.error(e);
        alert("Ошибка сети");
    }
}

async function registration(role) {
    if (role === "student") {
        await registrationStudent(fullNameInput.value, loginInput.value, passwordInput.value);
    }
    else if (role === "administrator") {
        await registrationAdministrator(fullNameInput.value, loginInput.value, passwordInput.value);
    }
    else if (role === "cook") {
        await registrationCook(fullNameInput.value, loginInput.value, passwordInput.value);
    }
}

registrationButton.addEventListener('click', async function () {
    const role = roleSelect.value;
    const code = codeInput.value;
    const password = passwordInput.value;
    const repPassword = repeatPasswordInput.value;
    const login = loginInput.value;

    if (await fullCheck(role, code, password, repPassword, login)) {
        await registration(role);
    }
    else {
        alert("Регистрация не удалась. Проверьте введённые данные.");
    }
});
