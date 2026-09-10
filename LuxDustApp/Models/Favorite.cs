namespace LuxDustApp.Models
{
	public class Favorite
	{
		public int Id { get; set; }
		public int UserId { get; set; }  
		public int ProductId { get; set; }
		public DateTime AddedAt { get; set; } = DateTime.UtcNow;
		public User User { get; set; }
		public Product Product { get; set; }
	}
}