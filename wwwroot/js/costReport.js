document.addEventListener("DOMContentLoaded", loadCostReport);

async function loadCostReport() {
    const container = document.getElementById("repost-container") || document.getElementById("report-container");
    if (!container) return;

    container.innerHTML = 'Загрузка отчёта...';

    const now = new Date();
    const year = now.getFullYear();
    const month = now.getMonth();
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const periodStart = firstDay.toLocaleDateString('ru-RU');
    const periodEnd = lastDay.toLocaleDateString('ru-RU');
    const generationDate = new Date().toLocaleDateString('ru-RU');

    try {
        const response = await fetch('/api/report/cost');
        if (!response.ok) {
            container.innerHTML = `<p>Ошибка загрузки: ${response.status} ${response.statusText}</p>`;
            return;
        }

        const report = await response.json();


        container.innerHTML = `
            <div class="report">
              <h2>ОТЧЁТ ПО ЗАТРАТАМ</h2>

              <div class="period">
                За период с <strong>${periodStart}</strong> по <strong>${periodEnd}</strong>
              </div>

              <p>Итоги по закупкам за период:</p>

              <ol>
                <li style="margin-bottom:8px;">
                  <strong>Всего куплено продуктов:</strong>
                  <div style="margin-top:6px;"><strong>${report.totalProduct}</strong> ед.</div>
                </li>

                <li>
                  <strong>Всего потрачено</strong>
                  <div style="margin-top:6px;"><strong>${report.totalSpent}</strong> р.</div>
                </li>
              </ol>

              <div class="signature" style="margin-top:18px;">
                <div>
                  Ответственное лицо: __________________ / <strong>[ФИО]</strong> /
                </div>
                <div style="margin-top:8px;">
                  Дата составления: <strong>${generationDate}</strong>
                </div>
              </div>
            </div>
        `;
    } catch (err) {
        console.error('Ошибка при получении отчёта затрат:', err);
        container.innerHTML = `<p style="color:red;">Ошибка при загрузке отчёта.</p>`;
    }
}