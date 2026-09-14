using System.Collections.Generic;

namespace LuxDustApp.Models
{
	public class AdminProductViewModel
	{
		public Product Product { get; set; }
		public List<string> Brands { get; set; }
		public List<string> Categories { get; set; }
		public Dictionary<string, List<string>> SubcategoriesByCategory { get; set; }
		public List<string> SkinTypes { get; set; }
		public List<string> Problems { get; set; }
		public List<string> Seasons { get; set; }
	}
}