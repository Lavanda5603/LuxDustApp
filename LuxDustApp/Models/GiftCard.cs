using System;

namespace LuxDustApp.Models
{
	public class GiftCard
	{
		public int Id { get; set; }
		public string Code { get; set; } = "";
		public int Amount { get; set; }
		public int RemainingAmount { get; set; }
		public bool IsActive { get; set; } = true;
		public int? OwnerUserId { get; set; }
		public int? UsedByUserId { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UsedAt { get; set; }
		public DateTime? ExpiresAt { get; set; }

		public User? OwnerUser { get; set; }
		public User? UsedByUser { get; set; }
	}
}