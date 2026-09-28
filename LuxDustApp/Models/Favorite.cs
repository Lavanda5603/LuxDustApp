// Favorite.cs - Модель избранного товара

using System;

namespace LuxDustApp.Models
{
	public class Favorite
	{
		public int Id { get; set; }

		// ID пользователя
		public int UserId { get; set; }

		// ID товара
		public int ProductId { get; set; }

		// Дата добавления в избранное
		public DateTime AddedAt { get; set; } = DateTime.UtcNow;

		// Навигационные свойства
		public User User { get; set; } = null!;
		public Product Product { get; set; } = null!;
	}
}