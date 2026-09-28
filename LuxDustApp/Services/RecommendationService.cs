// RecommendationService.cs - Алгоритм подбора косметики

using LuxDustApp.Data;
using LuxDustApp.Models;
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

		// Главный метод: принимает анкету, возвращает подборку с причинами
		public RecommendationBundle GetRecommendationsWithReasons(Profile profile)
		{
			_logger.LogInformation("Запуск подбора. Тип кожи: {SkinType}, Возраст: {Age}, Бюджет: {Budget}",
				profile.SkinType, profile.Age, profile.Budget);

			var allProducts = _context.Products.ToList();
			var result = new Dictionary<Product, RecommendationResult>();
			var bundle = new RecommendationBundle();

			if (allProducts == null || !allProducts.Any())
			{
				_logger.LogWarning("В базе нет товаров. Подборка не сформирована.");
				return bundle;
			}

			// Определяю, хочет ли пользователь уход за лицом
			bool userWantsFaceCare = !string.IsNullOrEmpty(profile.Goal) &&
				RecommendationConstants.FaceCareGoals.Any(g => profile.Goal.Contains(g));

			_logger.LogInformation("Пользователь хочет уход за лицом: {UserWantsFaceCare}", userWantsFaceCare);

			// Очищаю список аллергий от значения "Нет" (если пользователь случайно выбрал и то, и другое)
			var userAllergies = (profile.Allergies ?? "")
				.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.Select(a => a.Trim())
				.Where(a => a != "Нет")
				.ToList();

			foreach (var product in allProducts)
			{
				// Пропускаю товары из исключенных категорий
				if (!string.IsNullOrEmpty(product.Category) && RecommendationConstants.ExcludedCategories.Contains(product.Category))
					continue;

				// Если пользователь хочет уход за лицом, пропускаю неподходящие категории
				if (userWantsFaceCare && !string.IsNullOrEmpty(product.Category) &&
					RecommendationConstants.NonFaceCareCategories.Contains(product.Category))
					continue;

				var res = new RecommendationResult();
				var reasons = new List<string>();
				var warnings = new List<string>();

				// Прогоняю товар по всем 16 факторам
				CheckSkinType(profile, product, res, reasons, warnings);
				CheckAge(profile, product, res, reasons, warnings);
				CheckBudget(profile, product, res, reasons, warnings);
				CheckProblems(profile, product, res, reasons, warnings);
				CheckAllergies(profile, product, userAllergies, res, reasons, warnings);
				CheckSeason(profile, product, res, reasons, warnings);
				CheckBrands(profile, product, res, reasons, warnings);
				CheckGoal(profile, product, res, reasons, warnings);
				CheckStress(profile, product, res, reasons, warnings);
				CheckDiet(profile, product, res, reasons, warnings);
				CheckSunSensitivity(profile, product, res, reasons, warnings);
				CheckEdema(profile, product, res, reasons, warnings);
				CheckProfessionalCare(profile, product, res, reasons, warnings);
				CheckTexture(profile, product, res, reasons, warnings);
				CheckMultiStep(profile, product, res, reasons, warnings);
				CheckCategory(profile, product, res, reasons, warnings, userWantsFaceCare);
				CheckBonuses(product, res, reasons);

				// Формирую итоговый текст причин
				res.Reasons = reasons.Any() ? " ✓ " + string.Join("; ", reasons) + "." : "Нет явных совпадений.";
				if (warnings.Any())
					res.Reasons += " ! " + string.Join("; ", warnings) + ".";

				result[product] = res;
			}

			// Сортирую товары по убыванию рейтинга
			var sorted = result.OrderByDescending(kv => kv.Value.Score).ToList();

			// Формирую ТОП-10 и блок «Также может подойти»
			bundle.Top = sorted.Take(10).Select(kv => kv.Key).ToList();
			bundle.TopReasons = sorted.Take(10).ToDictionary(kv => kv.Key, kv => kv.Value);

			bundle.Middle = sorted.Skip(10).Take(50).Select(kv => kv.Key).ToList();
			bundle.MiddleReasons = sorted.Skip(10).Take(50).ToDictionary(kv => kv.Key, kv => kv.Value);

			_logger.LogInformation("Подборка сформирована. ТОП-10: {TopCount}, Также может подойти: {MiddleCount}",
				bundle.Top.Count, bundle.Middle.Count);

			return bundle;
		}

		// Проверяю, решает ли товар проблему пользователя (использую в нескольких местах)
		private bool SolvesAnyProblem(Profile profile, Product product)
		{
			if (string.IsNullOrEmpty(profile.Problems) || string.IsNullOrEmpty(product.Problem)) return false;

			var userProblems = profile.Problems.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.Select(p => p.Trim()).ToList();

			return userProblems.Any(p =>
				p == product.Problem ||
				(RecommendationConstants.ProblemRelations.ContainsKey(p) &&
				 RecommendationConstants.ProblemRelations[p].Contains(product.Problem)));
		}

		// Проверка типа кожи (40 баллов — самый важный фактор)
		private void CheckSkinType(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (string.IsNullOrEmpty(profile.SkinType) || string.IsNullOrEmpty(product.SkinType)) return;

			bool solvesProblem = SolvesAnyProblem(profile, product);

			if (profile.SkinType == product.SkinType)
			{ res.Score += 40; reasons.Add($"тип кожи «{profile.SkinType}»"); }
			else if (profile.SkinType == "Обезвоженная" && product.SkinType == "Сухая")
			{ res.Score += 25; reasons.Add("обезвоженная кожа требует увлажнения"); }
			else if (profile.SkinType == "Склонная к куперозу" && product.SkinType == "Чувствительная")
			{ res.Score += 25; reasons.Add("купероз требует бережного ухода"); }
			else if (profile.SkinType == "Комбинированная" && (product.SkinType == "Жирная" || product.SkinType == "Нормальная"))
			{ res.Score += 20; reasons.Add("комбинированная кожа включает зоны жирной/нормальной"); }
			else if (profile.SkinType == "Не знаю / хочу узнать" && product.SkinType == "Нормальная")
			{ res.Score += 10; reasons.Add("универсальное средство"); }
			else
			{
				if (solvesProblem) { res.Score -= 10; warnings.Add($"средство для «{product.SkinType}», но решает вашу проблему"); }
				else { res.Score -= 30; warnings.Add($"средство для «{product.SkinType}», а у вас «{profile.SkinType}»"); }
			}
		}

		// Проверка возраста
		private void CheckAge(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (profile.Age <= 0) return;

			if (profile.Age < 20)
			{
				if (product.Problem == "Акне") { res.Score += 20; reasons.Add("подходит для подростковой кожи"); }
				if (product.SubCategory == "Очищение (гели, пенки)") { res.Score += 10; reasons.Add("мягкое очищение"); }
			}
			else if (profile.Age < 30)
			{
				if (product.Problem == "Акне") { res.Score += 15; reasons.Add("подходит для молодой кожи"); }
				if (product.SubCategory == "Увлажнение") { res.Score += 10; reasons.Add("базовое увлажнение"); }
			}
			else if (profile.Age < 40)
			{
				if (product.Problem == "Акне") { res.Score += 10; reasons.Add("подходит для взрослой кожи с акне"); }
				if (product.Problem == "Тусклый цвет") { res.Score += 15; reasons.Add("борьба с тусклостью"); }
				if (product.Problem == "Морщины") { res.Score += 10; reasons.Add("профилактика морщин"); }
			}
			else if (profile.Age < 50)
			{
				if (product.SubCategory == "Антивозрастной уход") { res.Score += 25; reasons.Add("антивозрастной уход с 40 лет"); }
				if (product.Problem == "Морщины") { res.Score += 15; reasons.Add("активная борьба с морщинами"); }
			}
			else
			{
				if (product.SubCategory == "Антивозрастной уход") { res.Score += 30; reasons.Add("интенсивный антивозрастной уход"); }
				if (product.SubCategory == "Уход за глазами") { res.Score += 15; reasons.Add("уход за кожей вокруг глаз"); }
			}
		}

		// Проверка бюджета (с фильтром и штрафами)
		private void CheckBudget(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (profile.Budget <= 0) return;

			// Если товар дороже бюджета более чем в 2 раза — исключаю
			if (profile.Budget <= 1000 && product.Price > 2000)
			{ res.Score -= 100; warnings.Add($"цена {product.Price} руб. значительно выше бюджета"); return; }
			if (profile.Budget <= 3000 && product.Price > 6000)
			{ res.Score -= 100; warnings.Add($"цена {product.Price} руб. значительно выше бюджета"); return; }
			if (profile.Budget <= 10000 && product.Price > 20000)
			{ res.Score -= 100; warnings.Add($"цена {product.Price} руб. значительно выше бюджета"); return; }
			if (profile.Budget <= 25000 && product.Price > 50000)
			{ res.Score -= 100; warnings.Add($"цена {product.Price} руб. значительно выше бюджета"); return; }

			if (profile.Budget <= 1000)
			{
				if (product.Price <= 1000) { res.Score += 30; reasons.Add("бюджетный вариант"); }
				else
				{
					int overBudget = product.Price - 1000;
					int penalty = Math.Min(overBudget / 50, 50);
					res.Score -= penalty;
					warnings.Add($"цена {product.Price} руб. выше бюджета до 1000 руб.");
				}
			}
			else if (profile.Budget <= 3000)
			{
				if (product.Price <= 1000) { res.Score += 20; reasons.Add("экономный вариант"); }
				else if (product.Price <= 3000)
				{
					if (product.Price >= 2800) { res.Score += 20; reasons.Add("цена близко к верхней границе бюджета"); }
					else { res.Score += 30; reasons.Add("цена в бюджете 1000–3000"); }
				}
				else
				{
					int overBudget = product.Price - 3000;
					int penalty = Math.Min(overBudget / 100, 50);
					res.Score -= penalty;
					warnings.Add($"цена {product.Price} руб. выше бюджета");
				}
			}
			else if (profile.Budget <= 10000)
			{
				if (product.Price <= 3000) { res.Score += 20; reasons.Add("доступный вариант"); }
				else if (product.Price <= 10000) { res.Score += 30; reasons.Add("средний сегмент"); }
				else
				{
					int overBudget = product.Price - 10000;
					int penalty = Math.Min(overBudget / 500, 50);
					res.Score -= penalty;
					warnings.Add($"цена {product.Price} руб. выше среднего сегмента");
				}
			}
			else if (profile.Budget <= 25000)
			{
				if (product.Price <= 10000) { res.Score += 15; reasons.Add("доступный вариант"); }
				else if (product.Price <= 25000) { res.Score += 30; reasons.Add("премиум-сегмент"); }
				else
				{
					int overBudget = product.Price - 25000;
					int penalty = Math.Min(overBudget / 1000, 50);
					res.Score -= penalty;
					warnings.Add($"цена {product.Price} руб. выше бюджета");
				}
			}
			else
			{
				if (product.Rating >= 4.5)
				{
					res.Score += 20;
					reasons.Add("бюджет без ограничений");
					if (product.Rating >= 4.7) { res.Score += 5; reasons.Add("высокий рейтинг"); }
				}
				else
				{
					warnings.Add("низкий рейтинг при бюджете без ограничений");
				}
			}
		}

		// Проверка проблем (считаю только прямые решения, связанные — отдельно)
		private void CheckProblems(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (string.IsNullOrEmpty(profile.Problems) || string.IsNullOrEmpty(product.Problem)) return;

			var userProblems = profile.Problems.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.Select(p => p.Trim()).ToList();

			int directMatches = 0;
			int relatedMatches = 0;

			foreach (var problem in userProblems)
			{
				if (problem == product.Problem)
				{
					res.Score += 30;
					reasons.Add($"решает «{problem}»");
					directMatches++;

					if (userProblems.Count == 1)
					{
						res.Score += 10;
						reasons.Add("решает вашу главную проблему");
					}
				}
				else if (RecommendationConstants.ProblemRelations.ContainsKey(problem) &&
						 RecommendationConstants.ProblemRelations[problem].Contains(product.Problem))
				{
					res.Score += 15;
					reasons.Add($"связано с «{problem}»");
					relatedMatches++;
				}
			}

			// Бонус за количество прямых решений
			if (directMatches >= 2)
			{
				res.Score += 20;
				reasons.Add($"решает {directMatches} ваши проблемы");
			}

			if (directMatches == userProblems.Count && userProblems.Count > 1)
			{
				res.Score += 30;
				reasons.Add("решает все ваши проблемы");
			}

			// Если товар не решает ни одну проблему и его подкатегория не базовая — предупреждение
			if (directMatches == 0 && relatedMatches == 0 && userProblems.Any() &&
				!RecommendationConstants.BaseFaceSubCategories.Contains(product.SubCategory ?? ""))
			{
				warnings.Add($"не решает ваши проблемы («{string.Join(", ", userProblems)}»)");
			}
		}

		// Проверка аллергий (штраф за каждый аллерген + исключение при 2+)
		private void CheckAllergies(Profile profile, Product product, List<string> userAllergies, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (userAllergies == null || !userAllergies.Any()) return;

			int allergenCount = 0;

			foreach (var allergy in userAllergies)
			{
				if (RecommendationConstants.AllergenMap.ContainsKey(allergy) &&
					RecommendationConstants.AllergenMap[allergy](product))
				{
					res.Score -= 50;
					allergenCount++;
					warnings.Add($"содержит «{allergy}» — аллергия!");
				}
				else
				{
					res.Score += 10;
					reasons.Add($"без «{allergy}»");
				}
			}

			// Если аллергенов 2 и больше — исключаю товар
			if (allergenCount >= 2)
			{
				res.Score -= 100;
				warnings.Add($"содержит {allergenCount} аллергена — критично! Товар исключён.");
			}
		}

		// Проверка сезона
		private void CheckSeason(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (string.IsNullOrEmpty(profile.Season) || string.IsNullOrEmpty(product.Season)) return;

			if (product.Season == profile.Season) { res.Score += 15; reasons.Add($"сезон «{profile.Season}»"); }
			else if (product.Season == "Круглый год") { res.Score += 12; reasons.Add("подходит для всех сезонов"); }
			else if (profile.Season == "Круглый год") { res.Score += 8; reasons.Add($"подходит для «{product.Season}»"); }
			else
			{
				res.Score -= 20;
				warnings.Add($"средство для «{product.Season}», а у вас «{profile.Season}»");
			}
		}

		// Проверка любимых брендов
		private void CheckBrands(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (string.IsNullOrEmpty(profile.FavoriteBrands) || string.IsNullOrEmpty(product.Brand)) return;

			var userBrands = profile.FavoriteBrands.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.Select(b => b.Trim().ToLower()).ToList();
			var productBrand = product.Brand.ToLower();

			if (userBrands.Any(b => !string.IsNullOrEmpty(b) && (productBrand.Contains(b) || b.Contains(productBrand))))
			{
				res.Score += 25;
				reasons.Add($"бренд «{product.Brand}»");
			}
		}

		// Проверка цели ухода (с проверкой на противоположные цели)
		private void CheckGoal(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (string.IsNullOrEmpty(profile.Goal)) return;

			var userGoals = profile.Goal.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.Select(g => g.Trim()).ToList();

			// Проверяю противоположные цели
			bool wantsMatte = userGoals.Contains("Матирование");
			bool wantsNutrition = userGoals.Contains("Питание") || userGoals.Contains("Питание");

			if (wantsMatte && wantsNutrition)
			{
				// Штрафую товары, которые решают только одну из целей
				if (product.Problem == "Расширенные поры" && product.SubCategory == "Питание")
				{
					res.Score -= 10;
					warnings.Add("конфликт целей: матирование и питание");
				}
			}

			foreach (var goal in userGoals)
			{
				if (RecommendationConstants.GoalRules.ContainsKey(goal) &&
					RecommendationConstants.GoalRules[goal](product))
				{
					res.Score += 20;
					reasons.Add(goal.ToLower());
				}
			}
		}

		// Проверка уровня стресса
		private void CheckStress(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (profile.StressLevel == "Высокий")
			{
				if (product.Problem == "Гиперчувствительность") { res.Score += 15; reasons.Add("при стрессе"); }
				if (product.SubCategory == "Уход за глазами") { res.Score += 10; reasons.Add("от следов усталости"); }
			}
			else if (profile.StressLevel == "Средний")
			{
				if (product.Problem == "Тусклый цвет") { res.Score += 10; reasons.Add("при среднем стрессе"); }
				if (product.SubCategory == "Увлажнение") { res.Score += 5; reasons.Add("увлажнение при стрессе"); }
			}
		}

		// Проверка типа питания
		private void CheckDiet(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (profile.DietType == "Часто ем сладкое или жирное" && product.Problem == "Акне")
			{ res.Score += 15; reasons.Add("при высыпаниях из-за питания"); }
			else if (profile.DietType == "Вегетарианство" &&
				(product.Category == "Натуральная косметика" ||
				 (product.Description ?? "").ToLower().Contains("веган")))
			{ res.Score += 12; reasons.Add("для вегетарианцев"); }
		}

		// Проверка чувствительности к солнцу
		private void CheckSunSensitivity(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (!profile.SunSensitivity) return;

			if ((product.Name ?? "").ToLower().Contains("spf") || product.SubCategory == "Защита от солнца (SPF)")
			{ res.Score += 20; reasons.Add("SPF-защита"); }
			else
				warnings.Add("нет SPF — используйте отдельный солнцезащитный крем");
		}

		// Проверка склонности к отёкам
		private void CheckEdema(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (!profile.TendencyToEdema) return;

			if (product.Problem == "Отечность") { res.Score += 20; reasons.Add("борется с отёками"); }
			if (product.SubCategory == "Уход за глазами") { res.Score += 10; reasons.Add("от отёков под глазами"); }
		}

		// Проверка профессионального ухода
		private void CheckProfessionalCare(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (!profile.HasProfessionalCare) return;

			if (product.SubCategory == "Скрабы и пилинги")
			{ res.Score -= 10; warnings.Add("скрабы могут быть избыточны"); }
			if (product.Problem == "Гиперчувствительность") { res.Score += 10; reasons.Add("после проф. ухода"); }
		}

		// Проверка текстуры
		private void CheckTexture(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (string.IsNullOrEmpty(profile.TexturePreference) || string.IsNullOrEmpty(product.TexturePreference)) return;

			if (profile.TexturePreference == product.TexturePreference)
			{ res.Score += 10; reasons.Add($"текстура «{profile.TexturePreference}»"); }
		}

		// Проверка готовности к многоступенчатому уходу
		private void CheckMultiStep(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (profile.ReadyForMultiStep)
			{
				if (product.SubCategory == "Сыворотки и эссенции") { res.Score += 15; reasons.Add("для многоступенчатого ухода"); }
				if (product.SubCategory == "Тоники и лосьоны") { res.Score += 10; reasons.Add("доп. шаг ухода"); }
			}
			else
			{
				if (product.SubCategory == "Сыворотки и эссенции")
				{
					res.Score -= 50;
					warnings.Add("требует многоступенчатого ухода, а вы не готовы");
				}
			}
		}

		// Проверка категории
		private void CheckCategory(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings, bool userWantsFaceCare)
		{
			if (string.IsNullOrEmpty(product.Category)) return;

			var extraSubCategories = new List<string> { "Маски для лица", "Уход за глазами", "Скрабы и пилинги" };

			if (product.Category == "Уход за лицом" && userWantsFaceCare)
			{
				if (!extraSubCategories.Contains(product.SubCategory ?? ""))
				{
					res.Score += 15;
					reasons.Add("уход за лицом");
				}
				else
				{
					res.Score -= 10;
				}
			}
		}

		// Бонусы за новинку, акцию и высокий рейтинг
		private void CheckBonuses(Product product, RecommendationResult res, List<string> reasons)
		{
			if (product.IsNew) { res.Score += 5; reasons.Add("новинка"); }
			if (product.IsOnSale) { res.Score += 5; reasons.Add("акция"); }
			if (product.Rating >= 4.5) { res.Score += 5; reasons.Add($"рейтинг {product.Rating}"); }
		}
	}
}