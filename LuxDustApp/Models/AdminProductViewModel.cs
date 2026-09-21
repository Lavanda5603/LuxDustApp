using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace LuxDustApp.Models
{
	public class AdminProductViewModel
	{
		public Product Product { get; set; } = new Product();
		public List<string>? Brands { get; set; }
		public List<string>? Categories { get; set; }
		public Dictionary<string, List<string>>? SubcategoriesByCategory { get; set; }
		public List<string>? SkinTypes { get; set; }
		public List<string>? Problems { get; set; }
		public List<string>? Seasons { get; set; }
		public IFormFile? ImageFile { get; set; }
		public bool RemoveImage { get; set; }
	}
}