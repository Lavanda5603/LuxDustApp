(function () {
    const nextBtn = document.getElementById('nextBtn');
    if (!nextBtn) return;

    let currentStep = 1;
    const totalSteps = 15;
    const progressBar = document.getElementById('progressBar');
    const stepIndicator = document.getElementById('stepIndicator');
    const quizForm = document.getElementById('quizForm');

    function setupOtherOption(selectId, inputId) {
        const select = document.getElementById(selectId);
        const input = document.getElementById(inputId);
        if (!select || !input) return;

        select.addEventListener('change', function () {
            if (this.value === 'Другое (...)') {
                input.style.display = 'block';
                input.focus();
            } else {
                input.style.display = 'none';
                input.value = '';
            }
        });
    }

    setupOtherOption('problems', 'problemsOther');
    setupOtherOption('allergies', 'allergiesOther');

    function validateCurrentStep() {
        const currentStepEl = document.querySelector(`.step[data-step="${currentStep}"]`);
        if (!currentStepEl) return true;

        const fields = currentStepEl.querySelectorAll('select, input[type="text"]');
        let isValid = true;

        fields.forEach(field => {
            if (field.offsetParent === null) return;
            if (field.hasAttribute('required') || field.tagName === 'SELECT') {
                if (!field.value || field.value.trim() === '') {
                    field.style.border = '2px solid red';
                    isValid = false;
                } else {
                    field.style.border = '';
                }
            }
        });

        if (!isValid) {
            alert('Пожалуйста, заполните все поля на этом шаге!');
        }

        return isValid;
    }

    function updateStep() {
        document.querySelectorAll('.step').forEach(el => el.style.display = 'none');

        const currentStepEl = document.querySelector(`.step[data-step="${currentStep}"]`);
        if (currentStepEl) currentStepEl.style.display = 'block';

        if (progressBar) {
            const progressPercent = (currentStep / totalSteps) * 100;
            progressBar.style.width = progressPercent + '%';
            progressBar.setAttribute('aria-valuenow', progressPercent);
        }

        if (stepIndicator) {
            stepIndicator.textContent = `Шаг ${currentStep} из ${totalSteps}`;
        }

        if (currentStep === totalSteps) {
            nextBtn.textContent = 'Получить подборку';
        } else {
            nextBtn.textContent = 'Продолжить';
        }
    }

    nextBtn.addEventListener('click', function () {
        if (!validateCurrentStep()) return;

        if (currentStep < totalSteps) {
            currentStep++;
            updateStep();
        } else {
            quizForm.submit();
        }
    });

    updateStep();
})();

function addToCart(productId, btn) {
    fetch('/Quiz/AddToCart?productId=' + productId)
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                btn.textContent = 'Добавлено ✓';
                btn.disabled = true;
            } else {
                alert('Ошибка: ' + data.message);
            }
        })
        .catch(() => alert('Не удалось добавить товар'));
}

function confirmRemove(cartId) {
    if (confirm('Вы уверены, что хотите удалить этот товар из корзины?')) {
        window.location.href = '/Quiz/RemoveFromCart?cartId=' + cartId;
    }
}