function copyGiftCode() {
    var codeEl = document.getElementById('giftCardCode');
    if (!codeEl) return;

    var code = codeEl.textContent.trim();

    navigator.clipboard.writeText(code).then(function () {
        alert('Код скопирован: ' + code);
    });
}