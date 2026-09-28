// Product.cs - Модель косметического продукта

using System;

namespace LuxDustApp.Models
{
	public class Product
	{
		public int Id { get; set; }

		// Название товара
		public string? Name { get; set; }

		// Бренд
		public string? Brand { get; set; }

		// Категория («Уход за лицом»)
		public string? Category { get; set; }

		// Подкатегория («Сыворотки и эссенции»)
		public string? SubCategory { get; set; }

		// Цена
		public int Price { get; set; }

		// Тип кожи, для которого подходит
		public string? SkinType { get; set; }

		// Проблема, которую решает
		public string? Problem { get; set; }

		// Флаг «без аллергенов»
		public bool AllergenFree { get; set; }

		// Сезон
		public string? Season { get; set; }

		// Путь к изображению
		public string? ImageUrl { get; set; }

		// Описание
		public string? Description { get; set; }

		// Рейтинг (по умолчанию 4.5)
		public double Rating { get; set; }

		// Флаг «Новинка»
		public bool IsNew { get; set; }

		// Флаг «Акция»
		public bool IsOnSale { get; set; }

		// Дата добавления
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		// Чувствительность к солнцу
		public bool SunSensitivity { get; set; }

		// Склонность к отёкам
		public bool TendencyToEdema { get; set; }

		// Предпочтение по текстуре (Лёгкая / Плотная)
		public string? TexturePreference { get; set; }

		// Профессиональный уход
		public bool HasProfessionalCare { get; set; }

		// Готовность к многоступенчатому уходу
		public bool ReadyForMultiStep { get; set; }

		// Флаги аллергенов

		public bool HasParabens { get; set; } = false;
		public bool HasFragrance { get; set; } = false;
		public bool HasAlcohol { get; set; } = false;
		public bool HasSilicones { get; set; } = false;
		public bool HasSulfates { get; set; } = false;
		public bool HasGluten { get; set; } = false;
		public bool HasNuts { get; set; } = false;
		public bool HasEssentialOils { get; set; } = false;
	}
}