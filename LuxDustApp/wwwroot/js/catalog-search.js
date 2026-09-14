document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.getElementById('searchInput');
    const minPrice = document.getElementById('minPrice');
    const maxPrice = document.getElementById('maxPrice');
    const currentCategory = document.getElementById('currentCategory');
    const currentSubcategory = document.getElementById('currentSubcategory');
    const productsContainer = document.getElementById('productsContainer');

    if (!searchInput) return;

    function updateProducts() {
        const query = searchInput.value;
        const min = minPrice.value;
        const max = maxPrice.value;
        const category = currentCategory ? currentCategory.value : '';
        const subcategory = currentSubcategory ? currentSubcategory.value : '';

        const url = `/Catalog/Search?query=${encodeURIComponent(query)}&minPrice=${min}&maxPrice=${max}&category=${encodeURIComponent(category)}&subcategory=${encodeURIComponent(subcategory)}`;

        fetch(url)
            .then(response => response.text())
            .then(html => {
                productsContainer.innerHTML = html;
            })
            .catch(error => console.error('Ошибка поиска:', error));
    }

    searchInput.addEventListener('input', updateProducts);

    minPrice.addEventListener('input', updateProducts);
    maxPrice.addEventListener('input', updateProducts);
});