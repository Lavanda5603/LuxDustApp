(function () {
    const nextBtn = document.getElementById('nextBtn');
    const backBtn = document.getElementById('backBtn');
    if (!nextBtn) return;

    let currentStep = 1;
    const totalSteps = 15;
    const progressBar = document.getElementById('progressBar');
    const stepIndicator = document.getElementById('stepIndicator');
    const quizForm = document.getElementById('quizForm');

    function updateStep() {
        document.querySelectorAll('.step').forEach(el => el.style.display = 'none');
        const currentStepEl = document.querySelector(`.step[data-step="${currentStep}"]`);
        if (currentStepEl) currentStepEl.style.display = 'block';

        if (progressBar) progressBar.style.width = (currentStep / totalSteps) * 100 + '%';
        if (stepIndicator) stepIndicator.textContent = `Шаг ${currentStep} из ${totalSteps}`;
        nextBtn.textContent = (currentStep === totalSteps) ? 'Получить подборку' : 'Продолжить';
        if (backBtn) backBtn.style.display = (currentStep > 1) ? 'inline-block' : 'none';
    }

    nextBtn.addEventListener('click', function () {
        if (currentStep < totalSteps) {
            currentStep++;
            updateStep();
        } else {
            const formData = new FormData(quizForm);
            const problems = formData.getAll('Problems').join(',');
            const allergies = formData.getAll('Allergies').join(',');
            const goals = formData.getAll('Goal').join(',');

            quizForm.querySelectorAll('input[type="hidden"]').forEach(el => el.remove());

            [['Problems', problems], ['Allergies', allergies], ['Goal', goals]].forEach(([name, value]) => {
                const input = document.createElement('input');
                input.type = 'hidden';
                input.name = name;
                input.value = value;
                quizForm.appendChild(input);
            });

            quizForm.submit();
        }
    });

    if (backBtn) {
        backBtn.addEventListener('click', function () {
            if (currentStep > 1) {
                currentStep--;
                updateStep();
            }
        });
    }

    updateStep();
})();

function addToCart(productId, btn) {
    fetch('/Quiz/AddToCart?productId=' + productId)
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                btn.textContent = 'Добавлено';
                btn.disabled = true;
            } else {
                alert('Ошибка: ' + (data.message || 'Не удалось добавить'));
            }
        })
        .catch(() => alert('Не удалось добавить товар'));
}

function addToFavorites(productId, btn) {
    fetch('/Quiz/AddToFavorites?productId=' + productId)
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                btn.textContent = 'Добавлено';
                btn.disabled = true;
            } else {
                alert('Ошибка: ' + (data.message || 'Не удалось добавить'));
            }
        })
        .catch(() => alert('Не удалось добавить в избранное'));
}

function confirmRemove(cartId) {
    if (confirm('Вы уверены, что хотите удалить этот товар из корзины?')) {
        window.location.href = '/Quiz/RemoveFromCart?cartId=' + cartId;
    }
}

function updateQuantity(cartId, delta, btn) {
    fetch('/Quiz/UpdateQuantity?cartId=' + cartId + '&delta=' + delta, {
        method: 'POST'
    })
        .then(response => response.json())
        .then(data => {
            if (!data.success) {
                alert('Не удалось обновить количество');
                return;
            }

            document.querySelectorAll('.qty-value[data-cart-id="' + cartId + '"]').forEach(el => {
                el.textContent = data.quantity;
            });

            document.querySelectorAll('.item-total[data-cart-id="' + cartId + '"]').forEach(el => {
                const price = parseInt(el.dataset.price);
                el.textContent = (price * data.quantity) + ' руб.';
            });

            let grandTotal = 0;
            document.querySelectorAll('.item-total').forEach(el => {
                const price = parseInt(el.dataset.price);
                const qtyEl = document.querySelector('.qty-value[data-cart-id="' + el.dataset.cartId + '"]');
                const qty = parseInt(qtyEl.textContent);
                grandTotal += price * qty;
            });

            const grandTotalEl = document.getElementById('cartGrandTotal');
            if (grandTotalEl) grandTotalEl.textContent = grandTotal + ' руб.';

            const totalPriceEl = document.getElementById('totalPrice');
            const deliveryPriceEl = document.getElementById('deliveryPrice');
            if (totalPriceEl) {
                const delivery = deliveryPriceEl ? parseInt(deliveryPriceEl.textContent) : 0;
                totalPriceEl.textContent = grandTotal + delivery;
            }

            const varTotal = document.getElementById('varTotal');
            if (varTotal) varTotal.textContent = grandTotal;
        })
        .catch(() => alert('Ошибка при обновлении'));
}

function toggleAddress(show) {
    var addressBlock = document.getElementById('addressBlock');
    if (addressBlock) {
        addressBlock.style.display = show ? 'block' : 'none';
    }

    var deliveryPriceEl = document.getElementById('deliveryPrice');
    var varTotalEl = document.getElementById('varTotal');
    var totalPriceEl = document.getElementById('totalPrice');

    var delivery = show ? 300 : 0;
    if (deliveryPriceEl) deliveryPriceEl.textContent = delivery;

    var total = varTotalEl ? parseInt(varTotalEl.textContent) : 0;
    if (totalPriceEl) totalPriceEl.textContent = total + delivery;
}