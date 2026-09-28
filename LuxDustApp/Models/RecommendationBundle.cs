// RecommendationBundle.cs - Результат подбора: ТОП-10 + блок «Также может подойти»

using System.Collections.Generic;

namespace LuxDustApp.Models
{
	public class RecommendationBundle
	{
		// ТОП-10 лучших товаров
		public List<Product> Top { get; set; } = new List<Product>();

		// Причины для ТОП-10
		public Dictionary<Product, RecommendationResult> TopReasons { get; set; } = new Dictionary<Product, RecommendationResult>();

		// Блок «Также может подойти» (следующие 50 товаров)
		public List<Product> Middle { get; set; } = new List<Product>();

		// Причины для блока «Также может подойти»
		public Dictionary<Product, RecommendationResult> MiddleReasons { get; set; } = new Dictionary<Product, RecommendationResult>();
	}
}