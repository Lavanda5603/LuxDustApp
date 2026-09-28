// product-details.js - Просмотр фото товара в модальном окне

function openImage(src) {
    const modal = document.getElementById('imageModal');
    const modalImg = document.getElementById('imageModalContent');

    if (!modal || !modalImg) return;

    modal.style.display = 'flex';
    modalImg.src = src;

    // Блокирую прокрутку страницы
    document.body.style.overflow = 'hidden';
}

function closeImage() {
    const modal = document.getElementById('imageModal');
    if (!modal) return;

    modal.style.display = 'none';
    document.body.style.overflow = '';
}

// Закрытие по нажатию Escape
document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') closeImage();
});