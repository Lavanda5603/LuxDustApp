// site.js - PWA, активация ссылок в навбаре

let deferredPrompt;
const installBtn = document.getElementById('installPwaBtn');

// Событие "можно установить PWA" (Chrome, Android)
window.addEventListener('beforeinstallprompt', (e) => {
    e.preventDefault();
    deferredPrompt = e;
    if (installBtn) installBtn.style.display = 'inline-block';
});

// Установка PWA по кнопке
if (installBtn) {
    installBtn.addEventListener('click', async () => {
        if (deferredPrompt) {
            deferredPrompt.prompt();
            const { outcome } = await deferredPrompt.userChoice;
            deferredPrompt = null;
            installBtn.style.display = 'none';
        }
    });
}

// PWA: регистрация Service Worker
if ('serviceWorker' in navigator) {
    navigator.serviceWorker.register('/service-worker.js')
        .then(() => console.log('Service Worker зарегистрирован'))
        .catch((err) => console.warn('Ошибка регистрации Service Worker:', err));
}

// Подсветка активной ссылки в навбаре
document.addEventListener('DOMContentLoaded', function () {
    const path = window.location.pathname.toLowerCase();

    document.querySelectorAll('.navbar-nav .nav-link').forEach(function (link) {
        const href = link.getAttribute('href').toLowerCase();

        if (href === '/' && (path === '/' || path === '/home' || path === '/home/index')) {
            link.classList.add('active');
        } else if (href !== '/' && path.startsWith(href)) {
            link.classList.add('active');
        }
    });
});