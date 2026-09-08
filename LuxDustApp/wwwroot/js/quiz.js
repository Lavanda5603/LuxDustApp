(function () {
    const nextBtn = document.getElementById('nextBtn');
    if (!nextBtn) return;

    let currentStep = 1;
    const totalSteps = 8;
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
        if (currentStep < totalSteps) {
            currentStep++;
            updateStep();
        } else {
            quizForm.submit();
        }
    });

    updateStep();
})();