// home.js - Анимации главной страницы (появление блоков, счётчики, слайдеры)

document.addEventListener('DOMContentLoaded', function () {
    // Появление блоков при скролле
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
            }
        });
    }, { threshold: 0.1 });

    document.querySelectorAll('.fade-in').forEach(el => observer.observe(el));

    // Анимация цифр в блоке статистики
    const statsBlock = document.querySelector('.stats-block');
    if (statsBlock) {
        const statsObserver = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    animateNumbers();
                    statsObserver.disconnect();
                }
            });
        }, { threshold: 0.5 });
        statsObserver.observe(statsBlock);
    }

    // Горизонтальный скролл карточек (перетаскивание мышью и тачем)
    const sliders = document.querySelectorAll('.promo-scroll');
    sliders.forEach(slider => {
        let isDown = false;
        let startX;
        let scrollLeft;
        let hasMoved = false;

        // Мышь: начало перетаскивания
        slider.addEventListener('mousedown', (e) => {
            isDown = true;
            hasMoved = false;
            slider.classList.add('dragging');
            startX = e.pageX - slider.offsetLeft;
            scrollLeft = slider.scrollLeft;
        });

        // Мышь: конец перетаскивания
        slider.addEventListener('mouseleave', () => {
            isDown = false;
            slider.classList.remove('dragging');
        });

        slider.addEventListener('mouseup', () => {
            isDown = false;
            slider.classList.remove('dragging');
        });

        // Мышь: движение
        slider.addEventListener('mousemove', (e) => {
            if (!isDown) return;
            const x = e.pageX - slider.offsetLeft;
            const walk = (x - startX) * 1.5;

            if (Math.abs(walk) > 5) {
                hasMoved = true;
                e.preventDefault();
            }

            if (hasMoved) {
                slider.scrollLeft = scrollLeft - walk;
            }
        });

        // Блокирую клик по карточке, если было перетаскивание
        slider.addEventListener('click', (e) => {
            if (hasMoved) {
                e.preventDefault();
                e.stopPropagation();
            }
        }, true);

        // Тач: начало
        slider.addEventListener('touchstart', (e) => {
            startX = e.touches[0].pageX - slider.offsetLeft;
            scrollLeft = slider.scrollLeft;
            hasMoved = false;
        }, { passive: true });

        // Тач: движение
        slider.addEventListener('touchmove', (e) => {
            const x = e.touches[0].pageX - slider.offsetLeft;
            const walk = (x - startX) * 1.5;

            if (Math.abs(walk) > 5) hasMoved = true;

            slider.scrollLeft = scrollLeft - walk;
        }, { passive: true });
    });
});

// Анимация чисел (счётчики в блоке статистики)
function animateNumbers() {
    document.querySelectorAll('.stat-number').forEach(el => {
        const target = parseInt(el.getAttribute('data-target'));
        let current = 0;
        const increment = target / 60;
        const timer = setInterval(() => {
            current += increment;
            if (current >= target) {
                el.textContent = target + (el.getAttribute('data-suffix') || '');
                clearInterval(timer);
            } else {
                el.textContent = Math.floor(current) + (el.getAttribute('data-suffix') || '');
            }
        }, 20);
    });
}