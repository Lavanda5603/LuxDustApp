// AdminProductViewModel.cs - Модель для формы товара в админ-панели

using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace LuxDustApp.Models
{
	public class AdminProductViewModel
	{
		// Сам товар
		public Product Product { get; set; } = new Product();

		// Список брендов для выпадающего списка
		public List<string>? Brands { get; set; }

		// Список категорий для выпадающего списка
		public List<string>? Categories { get; set; }

		// Подкатегории по категориям (для автоподстановки)
		public Dictionary<string, List<string>>? SubcategoriesByCategory { get; set; }

		// Справочник типов кожи
		public List<string>? SkinTypes { get; set; }

		// Справочник проблем
		public List<string>? Problems { get; set; }

		// Справочник сезонов
		public List<string>? Seasons { get; set; }

		// Загружаемый файл изображения
		public IFormFile? ImageFile { get; set; }

		// Галочка «Удалить текущее фото»
		public bool RemoveImage { get; set; }
	}
}