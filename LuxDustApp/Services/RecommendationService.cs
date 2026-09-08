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

		public List<Product> GetRecommendations(Profile profile)
		{
			var allProducts = _context.Products.ToList();

			if (allProducts == null || !allProducts.Any())
			{
				_logger.LogWarning("В базе данных нет продуктов. Добавьте их через админ-панель.");
				return new List<Product>();
			}

			var scores = new Dictionary<int, int>();

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

				if (product.AllergenFree && !string.IsNullOrEmpty(profile.Allergies))
					score += 15;

				if (!string.IsNullOrEmpty(product.Season) && product.Season == profile.Season)
					score += 10;

				scores[product.Id] = score;
			}

			var topProducts = allProducts.Where(p => scores.ContainsKey(p.Id)).OrderByDescending(p => scores[p.Id]).Take(5).ToList();

			_logger.LogInformation($"Алгоритм завершён. Найдено {topProducts.Count} продуктов.");

			return topProducts;
		}
	}
}