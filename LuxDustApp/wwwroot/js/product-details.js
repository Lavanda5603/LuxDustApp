function openImage(src) {
    const modal = document.getElementById('imageModal');
    const modalImg = document.getElementById('imageModalContent');
    modal.style.display = 'flex';
    modalImg.src = src;
    document.body.style.overflow = 'hidden';
}

function closeImage() {
    const modal = document.getElementById('imageModal');
    modal.style.display = 'none';
    document.body.style.overflow = '';
}

document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') closeImage();
});