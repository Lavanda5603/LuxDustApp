// admin-category.js - Автоподстановка подкатегорий в форме товара (админка)

document.addEventListener('DOMContentLoaded', function () {
    const categoryInput = document.querySelector('input[name="Product.Category"]');
    const subcategoryInput = document.querySelector('input[name="Product.SubCategory"]');
    const subcategoriesList = document.getElementById('subcategoriesList');

    // Если это не форма товара, выхожу
    if (!categoryInput || !subcategoryInput || !subcategoriesList) return;

    // Сохраняю все подкатегории изначально
    const allSubOptions = Array.from(subcategoriesList.querySelectorAll('option'));

    // Функция фильтрации подкатегорий по выбранной категории
    function filterSubcategories() {
        const selectedCategory = (categoryInput.value || '').trim().toLowerCase();

        subcategoriesList.innerHTML = '';

        // Если категория не выбрана, показываю все подкатегории
        if (!selectedCategory) {
            allSubOptions.forEach(opt => {
                subcategoriesList.appendChild(opt.cloneNode(true));
            });
            return;
        }

        // Фильтрую подкатегории по data-category
        const filtered = allSubOptions.filter(opt => {
            const cat = (opt.dataset.category || '').trim().toLowerCase();
            return cat === selectedCategory;
        });

        // Показываю отфильтрованные или все, если ничего не найдено
        const toShow = filtered.length > 0 ? filtered : allSubOptions;
        toShow.forEach(opt => {
            subcategoriesList.appendChild(opt.cloneNode(true));
        });
    }

    categoryInput.addEventListener('input', filterSubcategories);
    categoryInput.addEventListener('change', filterSubcategories);

    // Запускаю фильтрацию при загрузке страницы
    filterSubcategories();
});