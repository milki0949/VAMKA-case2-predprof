const loginButton = document.getElementById('login');
const registrationButton = document.getElementById('registration');
console.log("main.js loaded");

loginButton.addEventListener('click', function() {
    window.location.href = '/login.html';
});

registrationButton.addEventListener('click', function () {
    console.log("Registration button clicked");
    window.location.href = '/registration.html';
});