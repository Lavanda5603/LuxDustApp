function toggleSubs(id) {
    var el = document.getElementById('subs-' + id);
    if (el) {
        el.style.display = (el.style.display === 'none') ? 'block' : 'none';
    }
}