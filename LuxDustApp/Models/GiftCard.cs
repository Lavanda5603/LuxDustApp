// GiftCard.cs - Модель подарочной карты

using System;

namespace LuxDustApp.Models
{
	public class GiftCard
	{
		public int Id { get; set; }

		// Уникальный код карты (LUX-XXXX-XXXX-XXXX)
		public string Code { get; set; } = "";

		// Номинал карты
		public int Amount { get; set; }

		// Остаток на карте
		public int RemainingAmount { get; set; }

		// Активна ли карта
		public bool IsActive { get; set; } = true;

		// ID владельца карты (кто купил)
		public int? OwnerUserId { get; set; }

		// ID того, кто использовал карту
		public int? UsedByUserId { get; set; }

		// Дата создания
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		// Дата использования
		public DateTime? UsedAt { get; set; }

		// Срок действия
		public DateTime? ExpiresAt { get; set; }

		// Навигационные свойства
		public User? OwnerUser { get; set; }
		public User? UsedByUser { get; set; }
	}
}