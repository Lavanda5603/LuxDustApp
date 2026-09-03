namespace LuxDustApp.Models
{
	public class Recommendation
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int ProductId { get; set; }
		public int Score { get; set; }
		public DateTime RecommendedAt { get; set; } = DateTime.Now;
		public User User { get; set; }
		public Product Product { get; set; }
	}
}