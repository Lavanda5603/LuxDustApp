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