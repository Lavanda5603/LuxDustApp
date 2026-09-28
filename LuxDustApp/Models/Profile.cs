// Profile.cs - Модель анкеты пользователя (15 параметров)

using System;

namespace LuxDustApp.Models
{
	public class Profile
	{
		public int Id { get; set; }

		// ID пользователя
		public int UserId { get; set; }

		// Тип кожи
		public string? SkinType { get; set; }

		// Возраст
		public int Age { get; set; }

		// Бюджет
		public int Budget { get; set; }

		// Проблемы кожи
		public string? Problems { get; set; }

		// Проблемы «Другое» (если выбрал «Другое»)
		public string? ProblemsOther { get; set; }

		// Аллергии
		public string? Allergies { get; set; }

		// Аллергии «Другое»
		public string? AllergiesOther { get; set; }

		// Сезон
		public string? Season { get; set; }

		// Любимые бренды
		public string? FavoriteBrands { get; set; }

		// Цель ухода
		public string? Goal { get; set; }

		// Уровень стресса
		public string? StressLevel { get; set; }

		// Тип питания
		public string? DietType { get; set; }

		// Чувствительность к солнцу
		public bool SunSensitivity { get; set; }

		// Склонность к отёкам
		public bool TendencyToEdema { get; set; }

		// Профессиональный уход
		public bool HasProfessionalCare { get; set; }

		// Предпочтение по текстуре
		public string? TexturePreference { get; set; }

		// Готовность к многоступенчатому уходу
		public bool ReadyForMultiStep { get; set; }

		// Дата последнего обновления анкеты
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		// Навигационное свойство
		public User? User { get; set; }
	}
}