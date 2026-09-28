// PromoCode.cs - Модель промокода

using System;

namespace LuxDustApp.Models
{
	public class PromoCode
	{
		public int Id { get; set; }

		// Код (SALE10)
		public string Code { get; set; } = "";

		// Процент скидки
		public int DiscountPercent { get; set; } = 0;

		// Фиксированная скидка
		public int FixedDiscount { get; set; } = 0;

		// Минимальная сумма заказа для применения
		public int MinOrderAmount { get; set; } = 0;

		// Активен ли промокод
		public bool IsActive { get; set; } = true;

		// Срок действия
		public DateTime? ExpiresAt { get; set; }

		// Максимум использований
		public int MaxUses { get; set; } = 0;

		// Сколько раз уже использован
		public int TimesUsed { get; set; } = 0;

		// Дата создания
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}