// OrderItem.cs - Модель товара в заказе

namespace LuxDustApp.Models
{
	public class OrderItem
	{
		public int Id { get; set; }

		// ID заказа
		public int OrderId { get; set; }

		// ID товара
		public int ProductId { get; set; }

		// Количество
		public int Quantity { get; set; }

		// Цена на момент заказа (сохраняю, чтобы не потерять при изменении цены товара)
		public int Price { get; set; }

		// Навигационные свойства
		public Order Order { get; set; } = null!;
		public Product Product { get; set; } = null!;
	}
}