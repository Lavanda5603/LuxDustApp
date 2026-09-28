// giftcard.js - Копирование кода подарочной карты

function copyGiftCode() {
    const codeEl = document.getElementById('giftCardCode');
    if (!codeEl) return;

    const code = codeEl.textContent.trim();

    // Копирую в буфер обмена
    navigator.clipboard.writeText(code).then(function () {
        alert('Код скопирован: ' + code);
    }).catch(function () {
        // Fallback для старых браузеров
        alert('Не удалось скопировать. Скопируйте вручную: ' + code);
    });
}