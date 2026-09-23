using System;
using System.Collections.Generic;

namespace LuxDustApp.Models
{
	public class Order
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public string? DeliveryMethod { get; set; }
		public string? Address { get; set; }
		public DateTime DeliveryDate { get; set; }
		public string? DeliveryTime { get; set; }
		public string? Phone { get; set; }
		public string? Comment { get; set; }
		public int TotalPrice { get; set; }
		public int DeliveryPrice { get; set; }
		public int Discount { get; set; }
		public string? PromoCode { get; set; }
		public int PromoDiscount { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public string? Status { get; set; }

		public List<OrderItem> Items { get; set; } = new List<OrderItem>();
		public List<OrderGiftCard> GiftCards { get; set; } = new List<OrderGiftCard>();
		public User? User { get; set; }
	}
}