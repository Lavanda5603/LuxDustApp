using System.Collections.Generic;

namespace LuxDustApp.Models
{
	public class RecommendationBundle
	{
		public List<Product> Top { get; set; } = new List<Product>();
		public Dictionary<Product, RecommendationResult> TopReasons { get; set; } = new Dictionary<Product, RecommendationResult>();
		public List<Product> Middle { get; set; } = new List<Product>();
		public Dictionary<Product, RecommendationResult> MiddleReasons { get; set; } = new Dictionary<Product, RecommendationResult>();
	}
}