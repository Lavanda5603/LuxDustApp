// promo.js - Промокоды и подарочные карты на странице оформления заказа

let appliedPromo = null;
let appliedGiftCards = [];

// Применяю промокод
function applyPromo() {
    const input = document.getElementById('promoCodeInput');
    const message = document.getElementById('promoMessage');

    if (!input || !message) return;

    const code = input.value.trim();

    if (!code) {
        message.textContent = 'Введите промокод';
        message.className = 'promo-message error';
        return;
    }

    const totalEl = document.getElementById('varTotal');
    const cartTotal = totalEl ? parseInt(totalEl.textContent) : 0;

    fetch('/Checkout/ApplyPromo', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: 'code=' + encodeURIComponent(code) + '&cartTotal=' + cartTotal
    })
        .then(r => r.json())
        .then(data => {
            if (!data.success) {
                message.textContent = data.message;
                message.className = 'promo-message error';
                appliedPromo = null;
                resetPromo();
                return;
            }

            // Промокод применён
            message.textContent = data.message;
            message.className = 'promo-message success';
            appliedPromo = data;

            const promoRow = document.getElementById('promoRow');
            if (promoRow) promoRow.style.display = 'flex';

            const promoDiscountEl = document.getElementById('promoDiscount');
            if (promoDiscountEl) promoDiscountEl.textContent = data.discount;

            const appliedPromoCodeEl = document.getElementById('appliedPromoCode');
            const appliedDiscountEl = document.getElementById('appliedDiscount');
            if (appliedPromoCodeEl) appliedPromoCodeEl.value = data.code;
            if (appliedDiscountEl) appliedDiscountEl.value = data.discount;

            recalcTotal();
        })
        .catch(() => {
            message.textContent = 'Ошибка при проверке промокода';
            message.className = 'promo-message error';
        });
}

// Сбрасываю промокод
function resetPromo() {
    const promoRow = document.getElementById('promoRow');
    if (promoRow) promoRow.style.display = 'none';

    const appliedPromoCodeEl = document.getElementById('appliedPromoCode');
    const appliedDiscountEl = document.getElementById('appliedDiscount');
    if (appliedPromoCodeEl) appliedPromoCodeEl.value = '';
    if (appliedDiscountEl) appliedDiscountEl.value = '0';

    recalcTotal();
}

// Пересчитываю итоговую сумму заказа
function recalcTotal() {
    const varTotalEl = document.getElementById('varTotal');
    const deliveryPriceEl = document.getElementById('deliveryPrice');
    const totalPriceEl = document.getElementById('totalPrice');
    const promoDiscountEl = document.getElementById('promoDiscount');
    const giftCardDiscountEl = document.getElementById('giftCardDiscount');

    const goods = varTotalEl ? parseInt(varTotalEl.textContent) : 0;
    const delivery = deliveryPriceEl ? parseInt(deliveryPriceEl.textContent) : 0;
    const promo = promoDiscountEl ? parseInt(promoDiscountEl.textContent) : 0;
    const gift = giftCardDiscountEl ? parseInt(giftCardDiscountEl.textContent) : 0;

    let total = goods + delivery - promo - gift;
    if (total < 0) total = 0;

    if (totalPriceEl) totalPriceEl.textContent = total;
}

// Применяю подарочную карту
function applyGiftCard() {
    const input = document.getElementById('giftCardInput');
    const message = document.getElementById('giftCardMessage');

    if (!input || !message) return;

    const code = input.value.trim().toUpperCase();

    if (!code) {
        message.textContent = 'Введите код карты';
        message.className = 'promo-message error';
        return;
    }

    // Проверяю, не применена ли уже эта карта
    if (appliedGiftCards.some(c => c.code === code)) {
        message.textContent = 'Эта карта уже применена';
        message.className = 'promo-message error';
        return;
    }

    const varTotalEl = document.getElementById('varTotal');
    const promoDiscountEl = document.getElementById('promoDiscount');
    const giftCardDiscountEl = document.getElementById('giftCardDiscount');

    const cartTotal = varTotalEl ? parseInt(varTotalEl.textContent) : 0;
    const promoDiscount = promoDiscountEl ? parseInt(promoDiscountEl.textContent) : 0;
    const alreadyApplied = giftCardDiscountEl ? parseInt(giftCardDiscountEl.textContent) : 0;

    fetch('/Checkout/ApplyGiftCard', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: 'code=' + encodeURIComponent(code) +
            '&cartTotal=' + cartTotal +
            '&promoDiscount=' + promoDiscount +
            '&alreadyApplied=' + alreadyApplied
    })
        .then(r => r.json())
        .then(data => {
            if (!data.success) {
                message.textContent = data.message;
                message.className = 'promo-message error';
                return;
            }

            // Карта применена
            message.textContent = '';
            appliedGiftCards.push({ code: data.code, amount: data.amount });

            renderAppliedGiftCards();

            const giftCardRow = document.getElementById('giftCardRow');
            if (giftCardRow) giftCardRow.style.display = 'flex';

            // Пересчитываю сумму всех применённых карт
            const totalApplied = appliedGiftCards.reduce((s, c) => s + c.amount, 0);
            if (giftCardDiscountEl) giftCardDiscountEl.textContent = totalApplied;

            // Скрытое поле для отправки на сервер
            const appliedInput = document.getElementById('appliedGiftCardsInput');
            if (appliedInput) appliedInput.value = appliedGiftCards.map(c => c.code).join(',');

            input.value = '';
            recalcTotal();
        })
        .catch(() => {
            message.textContent = 'Ошибка при проверке карты';
            message.className = 'promo-message error';
        });
}

// Отрисовываю список применённых подарочных карт
function renderAppliedGiftCards() {
    const container = document.getElementById('appliedGiftCards');
    if (!container) return;

    container.innerHTML = appliedGiftCards.map((c, i) => `
        <div class="applied-giftcard-item">
            <span class="applied-giftcard-code">${c.code}</span>
            <span class="applied-giftcard-amount">−${c.amount} руб.</span>
            <button type="button" class="applied-giftcard-remove" onclick="removeGiftCard(${i})">×</button>
        </div>
    `).join('');
}

// Удаляю подарочную карту из списка
function removeGiftCard(index) {
    appliedGiftCards.splice(index, 1);

    const giftCardDiscountEl = document.getElementById('giftCardDiscount');
    const totalApplied = appliedGiftCards.reduce((s, c) => s + c.amount, 0);
    if (giftCardDiscountEl) giftCardDiscountEl.textContent = totalApplied;

    // Скрываю строку "Подарочные карты", если их больше нет
    if (appliedGiftCards.length === 0) {
        const giftCardRow = document.getElementById('giftCardRow');
        if (giftCardRow) giftCardRow.style.display = 'none';
    }

    const appliedInput = document.getElementById('appliedGiftCardsInput');
    if (appliedInput) appliedInput.value = appliedGiftCards.map(c => c.code).join(',');

    renderAppliedGiftCards();
    recalcTotal();
}