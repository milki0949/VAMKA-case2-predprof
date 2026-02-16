tableBody = document.getElementById("table-body");

async function toggleLunch(studentId) {
    try {
        api = `api/student/update/lunch/${studentId}`;

        const response = await fetch(api, {
            method: 'POST'
        });
    } catch (e) {
        console.error("Ошибка сохранения", e);
        alert("Не удалось сохранить статус!");
    }
}

async function renderStudents() {
    try {
        api = 'api/student/lunch';

        const response = await fetch(api);

        if (!response.ok) throw new Error("Ошибка сервера");
        const students = await response.json();
        console.log(students)

        tableBody.innerHTML = '';

        students.forEach(student => {
            const row = document.createElement('tr');

            row.innerHTML = `
        <td class="col-id">${student.id}</td>
        <td class="col-name">${student.fullName}</td>
        <td class="col-issued">
            <input type="checkbox" 
                   onchange="toggleLunch(${student.id})">
        </td>`;

            tableBody.appendChild(row);
        });
    }
    catch (err) {
        tableBody.innerHTML = `<tr><td colspan="5" style="color:red">Ошибка: ${err.message}</td></tr>`;
    }
}

document.addEventListener('DOMContentLoaded', renderStudents);