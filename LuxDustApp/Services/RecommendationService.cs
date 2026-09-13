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

				if (!string.IsNullOrEmpty(product.SkinType) && !string.IsNullOrEmpty(profile.SkinType))
				{
					if (product.SkinType == profile.SkinType)
						score += 30;
					else if (profile.SkinType == "Обезвоженная" && product.SkinType == "Сухая")
						score += 15;
					else if (profile.SkinType == "Склонная к куперозу" && product.SkinType == "Чувствительная")
						score += 15;
					else if (profile.SkinType == "Комбинированная" && (product.SkinType == "Жирная" || product.SkinType == "Нормальная"))
						score += 10;
					else if (profile.SkinType == "Не знаю / хочу узнать" && product.SkinType == "Нормальная")
						score += 5;
				}

				if (profile.Budget > 0 && product.Price <= profile.Budget)
					score += 20;

				if (!string.IsNullOrEmpty(product.Problem) && !string.IsNullOrEmpty(profile.Problems) &&
					profile.Problems.Contains(product.Problem))
					score += 25;

				if (product.AllergenFree && !string.IsNullOrEmpty(profile.Allergies) && profile.Allergies != "Нет")
					score += 15;

				if (!string.IsNullOrEmpty(product.Season) && product.Season == profile.Season)
					score += 10;

				if (!string.IsNullOrEmpty(profile.FavoriteBrands) && !string.IsNullOrEmpty(product.Brand))
				{
					if (profile.FavoriteBrands.ToLower().Contains(product.Brand.ToLower()))
						score += 20;
				}

				if (!string.IsNullOrEmpty(profile.Goal))
				{
					if (profile.Goal == "Увлажнение" && (product.Name.Contains("увлажняющий") || product.Description.Contains("увлажняет")))
						score += 15;
					if (profile.Goal == "Питание" && (product.Name.Contains("питательный") || product.Description.Contains("питает")))
						score += 15;
					if (profile.Goal == "Антивозрастной" && product.SubCategory == "Антивозрастной уход")
						score += 15;
					if (profile.Goal == "Очищение" && product.SubCategory == "Очищение (гели, пенки)")
						score += 15;
					if (profile.Goal == "Защита" && product.Name.Contains("SPF"))
						score += 15;
					if (profile.Goal == "Матирование" && product.Problem == "Расширенные поры")
						score += 15;
					if (profile.Goal == "Лифтинг (подтяжка)" && product.Problem == "Морщины")
						score += 15;
					if (profile.Goal == "Осветление пигментации" && product.Problem == "Пигментация")
						score += 15;
					if (profile.Goal == "Сужение пор" && product.Problem == "Расширенные поры")
						score += 15;
					if (profile.Goal == "Сияние / здоровый вид" && product.Problem == "Тусклый цвет")
						score += 15;
					if (profile.Goal == "Снятие стресса и успокоение" && product.Problem == "Гиперчувствительность")
						score += 15;
				}

				if (profile.Age >= 40 && product.SubCategory == "Антивозрастной уход")
					score += 10;
				if (profile.Age >= 30 && product.Problem == "Морщины")
					score += 5;

				if (profile.StressLevel == "Высокий" &&
					(product.Problem == "Гиперчувствительность" || product.Name.Contains("успокаивающий")))
					score += 10;

				if (profile.DietType == "Часто ем сладкое или жирное" && product.Problem == "Акне")
					score += 10;

				if (profile.SunSensitivity && product.Name.Contains("SPF"))
					score += 10;

				if (profile.TendencyToEdema && product.Problem == "Отечность")
					score += 10;

				if (!string.IsNullOrEmpty(product.TexturePreference) && product.TexturePreference == profile.TexturePreference)
					score += 5;

				if (profile.ReadyForMultiStep &&
					(product.SubCategory == "Сыворотки и эссенции" || product.SubCategory == "Тоники и лосьоны"))
					score += 5;

				if (profile.HasProfessionalCare && product.Problem == "Гиперчувствительность")
					score += 5;

				if (product.IsNew) score += 3;
				if (product.IsOnSale) score += 2;
				if (product.Rating >= 4.5) score += 5;

				scores[product] = score;
			}

			var topProducts = scores
				.OrderByDescending(kv => kv.Value).Take(10).ToDictionary(kv => kv.Key, kv => kv.Value);

			_logger.LogInformation($"Алгоритм завершён. Найдено {topProducts.Count} продуктов.");

			return topProducts;
		}
	}
}