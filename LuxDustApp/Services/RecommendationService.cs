using LuxDustApp.Data;
using LuxDustApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuxDustApp.Services
{
	public class RecommendationService
	{
		private readonly ApplicationDbContext _context;
		private readonly ILogger<RecommendationService> _logger;

		public RecommendationService(ApplicationDbContext context, ILogger<RecommendationService> logger)
		{
			_context = context;
			_logger = logger;
		}

		public Dictionary<Product, int> GetRecommendationsWithScores(Profile profile)
		{
			var allProducts = _context.Products.ToList();

			if (allProducts == null || !allProducts.Any())
			{
				_logger.LogWarning("В базе данных нет продуктов.");
				return new Dictionary<Product, int>();
			}

			var scores = new Dictionary<Product, int>();

			foreach (var product in allProducts)
			{
				int score = 0;

				if (!string.IsNullOrEmpty(product.SkinType) && product.SkinType == profile.SkinType)
					score += 30;

				if (profile.Budget > 0 && product.Price <= profile.Budget)
					score += 20;

				if (!string.IsNullOrEmpty(product.Problem) && !string.IsNullOrEmpty(profile.Problems) &&
					profile.Problems.Contains(product.Problem))
					score += 25;

				if (product.AllergenFree && !string.IsNullOrEmpty(profile.Allergies) && profile.Allergies != "Нет")
					score += 15;

				if (!string.IsNullOrEmpty(product.Season) && product.Season == profile.Season)
					score += 10;

				if (profile.SunSensitivity && product.Name.Contains("SPF"))
					score += 10;

				if (profile.TendencyToEdema && product.Problem == "Отечность")
					score += 10;

				if (!string.IsNullOrEmpty(product.TexturePreference) && product.TexturePreference == profile.TexturePreference)
					score += 5;

				scores[product] = score;
			}

			var topProducts = scores.OrderByDescending(kv => kv.Value).Take(10).ToDictionary(kv => kv.Key, kv => kv.Value);

			_logger.LogInformation($"Алгоритм завершён. Найдено {topProducts.Count} продуктов.");

			return topProducts;
		}
	}
}