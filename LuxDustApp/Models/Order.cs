// Order.cs - Модель заказа

using System;
using System.Collections.Generic;

namespace LuxDustApp.Models
{
	public class Order
	{
		public int Id { get; set; }

		// ID пользователя, который сделал заказ
		public int UserId { get; set; }

		// Способ доставки: «Курьер» или «Самовывоз»
		public string? DeliveryMethod { get; set; }

		// Адрес доставки (только для курьера)
		public string? Address { get; set; }

		// Дата доставки
		public DateTime DeliveryDate { get; set; }

		// Время доставки (интервал)
		public string? DeliveryTime { get; set; }

		// Телефон для связи
		public string? Phone { get; set; }

		// Комментарий к заказу
		public string? Comment { get; set; }

		// Итоговая сумма заказа
		public int TotalPrice { get; set; }

		// Стоимость доставки
		public int DeliveryPrice { get; set; }

		// Общая скидка (промокод + подарочные карты)
		public int Discount { get; set; }

		// Применённый промокод
		public string? PromoCode { get; set; }

		// Скидка по промокоду
		public int PromoDiscount { get; set; }

		// Дата создания заказа
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		// Статус заказа (В обработке, Курьер в пути, Доставлен и т.д.)
		public string? Status { get; set; }

		// Навигационные свойства
		public List<OrderItem> Items { get; set; } = new List<OrderItem>();
		public List<OrderGiftCard> GiftCards { get; set; } = new List<OrderGiftCard>();
		public User? User { get; set; }
	}
}