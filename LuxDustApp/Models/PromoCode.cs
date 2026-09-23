using System;

namespace LuxDustApp.Models
{
	public class PromoCode
	{
		public int Id { get; set; }
		public string Code { get; set; } = "";
		public int DiscountPercent { get; set; } = 0;
		public int FixedDiscount { get; set; } = 0;
		public int MinOrderAmount { get; set; } = 0;
		public bool IsActive { get; set; } = true;
		public DateTime? ExpiresAt { get; set; }
		public int MaxUses { get; set; } = 0;
		public int TimesUsed { get; set; } = 0;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}