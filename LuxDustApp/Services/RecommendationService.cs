using LuxDustApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace LuxDustApp.Services
{
	public class RecommendationService
	{
		public List<Product> GetRecommendations()
		{
			return new List<Product>
			{
				new Product { Name = "Крем", Price = 1000, Brand = "Cream" },
				new Product { Name = "Гель", Price = 2000, Brand = "Gel" }
			};
		}
	}
}