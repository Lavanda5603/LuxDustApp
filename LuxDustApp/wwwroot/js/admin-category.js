document.addEventListener('DOMContentLoaded', function () {
    const categoryInput = document.querySelector('input[name="Product.Category"]');
    const subcategoryInput = document.querySelector('input[name="Product.SubCategory"]');
    const subcategoriesList = document.getElementById('subcategoriesList');

    if (!categoryInput || !subcategoryInput || !subcategoriesList) return;

    const allSubOptions = Array.from(subcategoriesList.querySelectorAll('option'));

    function filterSubcategories() {
        const selectedCategory = (categoryInput.value || '').trim().toLowerCase();

        subcategoriesList.innerHTML = '';

        if (!selectedCategory) {
            allSubOptions.forEach(opt => {
                subcategoriesList.appendChild(opt.cloneNode(true));
            });
            return;
        }

        const filtered = allSubOptions.filter(opt => {
            const cat = (opt.dataset.category || '').trim().toLowerCase();
            return cat === selectedCategory;
        });

        const toShow = filtered.length > 0 ? filtered : allSubOptions;
        toShow.forEach(opt => {
            subcategoriesList.appendChild(opt.cloneNode(true));
        });
    }

    categoryInput.addEventListener('input', filterSubcategories);
    categoryInput.addEventListener('change', filterSubcategories);

    filterSubcategories();
});