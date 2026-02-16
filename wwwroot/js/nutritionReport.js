const reportContainer = document.getElementById("report-container");
const now = new Date();
const year = now.getFullYear();
const month = now.getMonth();
const firstDay = new Date(year, month, 1);
const lastDay = new Date(year, month + 1, 0);

document.addEventListener("DOMContentLoaded", async function () {
    if (!reportContainer) return;

    try {
        const res = await fetch('/api/report/nutrition');
        if (!res.ok) {
            reportContainer.innerHTML = `<p>Ошибка загрузки: ${res.status} ${res.statusText}</p>`;
            return;
        }

        const report = await res.json();

        const periodStart = firstDay.toLocaleDateString('ru-RU');
        const periodEnd = lastDay.toLocaleDateString('ru-RU');
        const totalSold = report.totalSubscription;
        const breakfast = report.totalGotBreakfast;
        const lunch = report.totalGotLunch;
        const totalGiven = report.totalGotFood;
        const responsible = '[ФИО]';
        const generationDate = new Date().toLocaleDateString('ru-RU');

        reportContainer.innerHTML = `
            <div class="report">
              <h2>ОТЧЁТ ПО ПИТАНИЮ</h2>

              <div class="period">
                За период с <strong>${periodStart}</strong> по <strong>${periodEnd}</strong>
              </div>

              <p>
                Настоящий отчет составлен на основании данных системы учета и реализации абонементов. За отчетный период зафиксированы следующие показатели:
              </p>

              <ol>
                <li>
                  <strong>Реализация услуг</strong><br/>
                  Всего за указанный период было реализовано <strong>${totalSold}</strong> абонементов на питание.
                </li>

                <li style="margin-top:12px;">
                  <strong>Фактическое потребление</strong><br/>
                  Согласно данным операционного учета, фактическая выдача порций составила:
                  <ul>
                    <li>Завтраки: <strong>${breakfast}</strong> ед.</li>
                    <li>Обеды: <strong>${lunch}</strong> ед.</li>
                    <li>Всего отдано: <strong>${totalGiven}</strong> ед.</li>
                  </ul>
                </li>
              </ol>

              <div class="signature" style="margin-top:18px;">
                <div>
                  Ответственное лицо: __________________ / <strong>${responsible}</strong> /
                </div>
                <div style="margin-top:8px;">
                  Дата составления: <strong>${generationDate}</strong>
                </div>
              </div>
            </div>
        `;
    } catch (err) {
        console.error('Ошибка при получении отчёта:', err);
        reportContainer.innerHTML = `<p style="color:red;">Ошибка при загрузке отчёта.</p>`;
    }
});