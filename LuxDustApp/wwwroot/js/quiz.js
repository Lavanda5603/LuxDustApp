(function () {
    const nextBtn = document.getElementById('nextBtn');
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

        if (progressBar) {
            const progressPercent = (currentStep / totalSteps) * 100;
            progressBar.style.width = progressPercent + '%';
        }
        if (stepIndicator) {
            stepIndicator.textContent = `Шаг ${currentStep} из ${totalSteps}`;
        }
        nextBtn.textContent = (currentStep === totalSteps) ? 'Получить подборку' : 'Продолжить';
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

            const problemsInput = document.createElement('input');
            problemsInput.type = 'hidden';
            problemsInput.name = 'Problems';
            problemsInput.value = problems;
            quizForm.appendChild(problemsInput);

            const allergiesInput = document.createElement('input');
            allergiesInput.type = 'hidden';
            allergiesInput.name = 'Allergies';
            allergiesInput.value = allergies;
            quizForm.appendChild(allergiesInput);

            const goalInput = document.createElement('input');
            goalInput.type = 'hidden';
            goalInput.name = 'Goal';
            goalInput.value = goals;
            quizForm.appendChild(goalInput);

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