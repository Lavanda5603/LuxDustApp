// Recommendation.cs - Модель сохранённой рекомендации

using System;

namespace LuxDustApp.Models
{
	public class Recommendation
	{
		public int Id { get; set; }

		// ID пользователя
		public int UserId { get; set; }

		// ID товара
		public int ProductId { get; set; }

		// Рейтинг совместимости (балл)
		public int Score { get; set; }

		// Дата подбора
		public DateTime RecommendedAt { get; set; } = DateTime.UtcNow;

		// Навигационные свойства
		public User User { get; set; } = null!;
		public Product Product { get; set; } = null!;

		// Причины (почему товар подошёл)
		public string? Reasons { get; set; }
	}
}