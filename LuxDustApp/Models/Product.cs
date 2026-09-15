namespace LuxDustApp.Models
{
	public class Product
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? Brand { get; set; }
		public string? Category { get; set; }
		public string? SubCategory { get; set; }
		public int Price { get; set; }
		public string? SkinType { get; set; }
		public string? Problem { get; set; }
		public bool AllergenFree { get; set; }
		public string? Season { get; set; }
		public string? ImageUrl { get; set; }
		public string? Description { get; set; }
		public double Rating { get; set; }
		public bool IsNew { get; set; }
		public bool IsOnSale { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public bool SunSensitivity { get; set; }
		public bool TendencyToEdema { get; set; }
		public string? TexturePreference { get; set; }
		public bool HasProfessionalCare { get; set; }
		public bool ReadyForMultiStep { get; set; }

		public bool HasParabens { get; set; } = false;
		public bool HasFragrance { get; set; } = false;
		public bool HasAlcohol { get; set; } = false;
		public bool HasSilicones { get; set; } = false;
		public bool HasSulfates { get; set; } = false;
		public bool HasGluten { get; set; } = false;
		public bool HasNuts { get; set; } = false;
		public bool HasEssentialOils { get; set; } = false;
	}
}