// admin-search.js - Живой поиск и фильтрация в админ-панели

document.addEventListener('DOMContentLoaded', function () {
    // Нахожу все элементы фильтрации
    const searchInput = document.getElementById('adminSearchInput');
    const brandFilter = document.getElementById('adminBrandFilter');
    const categoryFilter = document.getElementById('adminCategoryFilter');
    const filterButtons = document.querySelectorAll('.admin-search-panel .filter-btn');
    const productsBody = document.getElementById('adminProductsBody');
    const foundCount = document.getElementById('adminFoundCount');

    // Если это не страница админки, выхожу
    if (!searchInput || !productsBody) return;

    let currentFilter = '';
    let debounceTimer = null;

    // Функция обновления таблицы через AJAX
    function updateProducts() {
        const query = searchInput.value.trim();
        const brand = brandFilter ? brandFilter.value : '';
        const category = categoryFilter ? categoryFilter.value : '';

        const url = `/Admin/Search?query=${encodeURIComponent(query)}&brand=${encodeURIComponent(brand)}&category=${encodeURIComponent(category)}&filter=${encodeURIComponent(currentFilter)}`;

        fetch(url)
            .then(r => r.text())
            .then(html => {
                productsBody.innerHTML = html;

                // Считаю количество найденных товаров
                const rows = productsBody.querySelectorAll('tr');
                const empty = productsBody.querySelector('td[colspan="6"]');
                if (foundCount) {
                    foundCount.textContent = empty ? 0 : rows.length;
                }
            })
            .catch(err => console.error('Ошибка поиска:', err));
    }

    // Живой поиск с задержкой (debounce) 200 мс
    searchInput.addEventListener('input', function () {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(updateProducts, 200);
    });

    // Фильтр по бренду и категории
    if (brandFilter) brandFilter.addEventListener('change', updateProducts);
    if (categoryFilter) categoryFilter.addEventListener('change', updateProducts);

    // Кнопки быстрых фильтров (Все / Новинки / Акции)
    filterButtons.forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            filterButtons.forEach(b => b.classList.remove('active'));
            this.classList.add('active');
            currentFilter = this.dataset.filter || '';
            updateProducts();
        });
    });
});