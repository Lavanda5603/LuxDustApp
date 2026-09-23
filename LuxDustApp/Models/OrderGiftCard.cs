namespace LuxDustApp.Models
{
	public class OrderGiftCard
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		public int GiftCardId { get; set; }
		public int AppliedAmount { get; set; }

		public Order? Order { get; set; }
		public GiftCard? GiftCard { get; set; }
	}
}