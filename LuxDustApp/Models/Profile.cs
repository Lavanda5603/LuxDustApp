namespace LuxDustApp.Models
{
	public class Profile
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public string? SkinType { get; set; }
		public int Age { get; set; }
		public int Budget { get; set; }
		public string? Problems { get; set; }
		public string? ProblemsOther { get; set; }
		public string? Allergies { get; set; }
		public string? AllergiesOther { get; set; }
		public string? Season { get; set; }
		public string? FavoriteBrands { get; set; }
		public string? Goal { get; set; }
		public string? StressLevel { get; set; }
		public string? DietType { get; set; }
		public bool SunSensitivity { get; set; }
		public bool TendencyToEdema { get; set; }
		public bool HasProfessionalCare { get; set; }
		public string? TexturePreference { get; set; }
		public bool ReadyForMultiStep { get; set; }
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
		public User? User { get; set; }
	}
}