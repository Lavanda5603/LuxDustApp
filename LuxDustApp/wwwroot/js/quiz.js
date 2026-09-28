// quiz.js - Логика пошаговой анкеты, корзины и оформления заказа

(function () {
    // Нахожу все необходимые элементы на странице
    const nextBtn = document.getElementById('nextBtn');
    const backBtn = document.getElementById('backBtn');

    // Если это не страница анкеты, выхожу из этого блока
    if (!nextBtn) return;

    // Нахожу все шаги анкеты
    const steps = document.querySelectorAll('.step');
    const totalSteps = steps.length;
    const progressBar = document.getElementById('progressBar');
    const stepIndicator = document.getElementById('stepIndicator');
    const quizForm = document.getElementById('quizForm');

    let currentStep = 1;

    // Функция обновления интерфейса при переходе на новый шаг
    function updateStep() {
        steps.forEach((step, index) => {
            step.style.display = (index + 1 === currentStep) ? 'block' : 'none';
        });

        if (progressBar) {
            progressBar.style.width = (currentStep / totalSteps) * 100 + '%';
        }

        if (stepIndicator) {
            stepIndicator.textContent = `Шаг ${currentStep} из ${totalSteps}`;
        }

        nextBtn.textContent = (currentStep === totalSteps) ? 'Получить подборку' : 'Продолжить';

        if (backBtn) {
            backBtn.style.display = (currentStep > 1) ? 'inline-block' : 'none';
        }
    }

    // Функция проверки, заполнен ли текущий шаг
    function isCurrentStepValid() {
        const currentStepEl = steps[currentStep - 1];

        // Если шаг помечен как необязательный, пропускаю его
        if (currentStepEl.dataset.optional === 'true') return true;

        const radioCheckboxInputs = currentStepEl.querySelectorAll('input[type="radio"], input[type="checkbox"]');
        const textInputs = currentStepEl.querySelectorAll('input[type="text"]');

        if (radioCheckboxInputs.length === 0 && textInputs.length === 0) return true;

        let hasValue = false;

        textInputs.forEach(input => {
            if (input.value.trim() !== '') hasValue = true;
        });

        radioCheckboxInputs.forEach(input => {
            if (input.checked) hasValue = true;
        });

        if (!hasValue) {
            alert('Пожалуйста, выберите хотя бы один вариант ответа или заполните поле.');
            return false;
        }

        return true;
    }

    // Обработчик кнопки "Продолжить"
    nextBtn.addEventListener('click', function () {
        if (!isCurrentStepValid()) return;

        if (currentStep < totalSteps) {
            currentStep++;
            updateStep();
        } else {
            submitQuiz();
        }
    });

    // Обработчик кнопки "Назад"
    if (backBtn) {
        backBtn.addEventListener('click', function () {
            if (currentStep > 1) {
                currentStep--;
                updateStep();
            }
        });
    }

    // Функция сбора данных и отправки формы
    function submitQuiz() {
        const formData = new FormData(quizForm);

        quizForm.querySelectorAll('input[type="hidden"]').forEach(el => el.remove());

        const multipleChoiceFields = ['Problems', 'Allergies', 'Goal'];

        multipleChoiceFields.forEach(fieldName => {
            const values = formData.getAll(fieldName);
            if (values.length > 0) {
                const hiddenInput = document.createElement('input');
                hiddenInput.type = 'hidden';
                hiddenInput.name = fieldName;
                hiddenInput.value = values.join(',');
                quizForm.appendChild(hiddenInput);
            }
        });

        quizForm.submit();
    }

    updateStep();
})();

// Функция добавления товара в корзину
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

// Функция добавления товара в избранное
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

// Функция удаления товара из корзины с подтверждением
function confirmRemove(cartId) {
    if (confirm('Вы уверены, что хотите удалить этот товар из корзины?')) {
        window.location.href = '/Quiz/RemoveFromCart?cartId=' + cartId;
    }
}

// Функция обновления количества товара в корзине
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

// Функция переключения отображения адреса доставки
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