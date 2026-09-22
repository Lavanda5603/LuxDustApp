document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.getElementById('adminSearchInput');
    const brandFilter = document.getElementById('adminBrandFilter');
    const categoryFilter = document.getElementById('adminCategoryFilter');
    const filterButtons = document.querySelectorAll('.admin-search-panel .filter-btn');
    const productsBody = document.getElementById('adminProductsBody');
    const foundCount = document.getElementById('adminFoundCount');

    if (!searchInput || !productsBody) return;

    let currentFilter = '';
    let debounceTimer = null;

    function updateProducts() {
        const query = searchInput.value.trim();
        const brand = brandFilter ? brandFilter.value : '';
        const category = categoryFilter ? categoryFilter.value : '';

        const url = `/Admin/Search?query=${encodeURIComponent(query)}&brand=${encodeURIComponent(brand)}&category=${encodeURIComponent(category)}&filter=${encodeURIComponent(currentFilter)}`;

        fetch(url)
            .then(r => r.text())
            .then(html => {
                productsBody.innerHTML = html;
                const rows = productsBody.querySelectorAll('tr');
                const empty = productsBody.querySelector('td[colspan="6"]');
                if (foundCount) {
                    foundCount.textContent = empty ? 0 : rows.length;
                }
            })
            .catch(err => console.error('Ошибка поиска:', err));
    }

    searchInput.addEventListener('input', function () {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(updateProducts, 200);
    });

    if (brandFilter) brandFilter.addEventListener('change', updateProducts);
    if (categoryFilter) categoryFilter.addEventListener('change', updateProducts);

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