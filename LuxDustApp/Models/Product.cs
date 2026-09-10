namespace LuxDustApp.Models
{
	public class Product
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Brand { get; set; }
		public string Category { get; set; }
		public int Price { get; set; }
		public string SkinType { get; set; }
		public string Problem { get; set; }
		public bool AllergenFree { get; set; }
		public string Season { get; set; }
		public string ImageUrl { get; set; }
		public string Description { get; set; }
		public double Rating { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public bool SunSensitivity { get; set; }
		public bool TendencyToEdema { get; set; }
		public string TexturePreference { get; set; }
		public bool HasProfessionalCare { get; set; }
		public bool ReadyForMultiStep { get; set; }
	}
}