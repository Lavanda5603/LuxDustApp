// OrderGiftCard.cs - Связь заказа и подарочной карты

namespace LuxDustApp.Models
{
	public class OrderGiftCard
	{
		public int Id { get; set; }

		// ID заказа
		public int OrderId { get; set; }

		// ID подарочной карты
		public int GiftCardId { get; set; }

		// Сумма, списанная с карты за этот заказ
		public int AppliedAmount { get; set; }

		// Навигационные свойства
		public Order? Order { get; set; }
		public GiftCard? GiftCard { get; set; }
	}
}