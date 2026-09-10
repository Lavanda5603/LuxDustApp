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