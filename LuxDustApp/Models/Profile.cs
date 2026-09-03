namespace LuxDustApp.Models
{
	public class Profile
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public string SkinType { get; set; } 
		public int Age { get; set; }
		public int Budget { get; set; }
		public string Problems { get; set; }
		public string Allergies { get; set; }
		public string Season { get; set; }
		public string FavoriteBrands { get; set; }
		public string Goal { get; set; }
		public DateTime UpdatedAt { get; set; } = DateTime.Now;
		public User User { get; set; }
	}
}