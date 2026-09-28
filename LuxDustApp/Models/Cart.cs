// Cart.cs - Модель товара в корзине

using System;

namespace LuxDustApp.Models
{
	public class Cart
	{
		public int Id { get; set; }

		// ID пользователя, которому принадлежит товар
		public int UserId { get; set; }

		// ID товара
		public int ProductId { get; set; }

		// Количество (по умолчанию 1)
		public int Quantity { get; set; } = 1;

		// Дата добавления в корзину
		public DateTime AddedAt { get; set; } = DateTime.UtcNow;

		// Навигационные свойства
		public User User { get; set; } = null!;
		public Product Product { get; set; } = null!;
	}
}