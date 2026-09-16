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

		public RecommendationBundle GetRecommendationsWithReasons(Profile profile)
		{
			var allProducts = _context.Products.ToList();
			var result = new Dictionary<Product, RecommendationResult>();
			var bundle = new RecommendationBundle();

			if (allProducts == null || !allProducts.Any())
				return bundle;

			var excludedCategories = new List<string> { "Подборки", "Уборка и стирка", "Хобби и творчество", "Фигура мечты" };

			bool userWantsFaceCare = !string.IsNullOrEmpty(profile.Goal) &&
				(profile.Goal.Contains("Увлажнение") || profile.Goal.Contains("Очищение") ||
				 profile.Goal.Contains("Антивозрастной") || profile.Goal.Contains("Питание") ||
				 profile.Goal.Contains("Сияние") || profile.Goal.Contains("Матирование") ||
				 profile.Goal.Contains("Сужение пор") || profile.Goal.Contains("Лифтинг") ||
				 profile.Goal.Contains("Осветление") || profile.Goal.Contains("Восстановление") ||
				 profile.Goal.Contains("Снятие стресса"));

			foreach (var product in allProducts)
			{
				if (!string.IsNullOrEmpty(product.Category) && excludedCategories.Contains(product.Category))
					continue;

				if (userWantsFaceCare &&
					(product.Category == "Уход за телом" ||
					 product.Category == "Волосы" ||
					 product.Category == "Макияж" ||
					 product.Category == "Для мужчин" ||
					 product.Category == "Для детей" ||
					 product.Category == "Здоровье и БАДы" ||
					 product.Category == "Парфюмерия" ||
					 product.Category == "Для дома" ||
					 product.Category == "Аксессуары" ||
					 product.Category == "Мини-форматы" ||
					 product.Category == "Маникюр и педикюр"))
					continue;

				var res = new RecommendationResult();
				var reasons = new List<string>();
				var warnings = new List<string>();

				CheckSkinType(profile, product, res, reasons, warnings);
				CheckAge(profile, product, res, reasons, warnings);
				CheckBudget(profile, product, res, reasons, warnings);
				CheckProblems(profile, product, res, reasons, warnings);
				CheckAllergies(profile, product, res, reasons, warnings);
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

				res.Reasons = reasons.Any() ? " ✓ " + string.Join("; ", reasons) + "." : "Нет явных совпадений.";
				if (warnings.Any())
					res.Reasons += " ! " + string.Join("; ", warnings) + ".";

				result[product] = res;
			}

			var sorted = result.OrderByDescending(kv => kv.Value.Score).ToList();

			bundle.Top = sorted.Take(10).Select(kv => kv.Key).ToList();
			bundle.TopReasons = sorted.Take(10).ToDictionary(kv => kv.Key, kv => kv.Value);

			bundle.Middle = sorted.Skip(10).Take(50).Select(kv => kv.Key).ToList();
			bundle.MiddleReasons = sorted.Skip(10).Take(50).ToDictionary(kv => kv.Key, kv => kv.Value);

			return bundle;
		}

		private void CheckSkinType(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (string.IsNullOrEmpty(profile.SkinType) || string.IsNullOrEmpty(product.SkinType)) return;

			bool solvesProblem = !string.IsNullOrEmpty(profile.Problems) &&
								 !string.IsNullOrEmpty(product.Problem) &&
								 profile.Problems.Split(',', StringSplitOptions.RemoveEmptyEntries)
									 .Select(p => p.Trim()).Contains(product.Problem);

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

		private void CheckBudget(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (profile.Budget <= 0) return;

			if (profile.Budget <= 1000)
			{
				if (product.Price <= 1000) { res.Score += 30; reasons.Add("бюджетный вариант"); }
				else warnings.Add($"цена {product.Price} руб. выше бюджета до 1000 руб.");
			}
			else if (profile.Budget <= 3000)
			{
				if (product.Price <= 1000) { res.Score += 20; reasons.Add("экономный вариант"); }
				else if (product.Price <= 3000)
				{
					if (product.Price >= 2800) { res.Score += 20; reasons.Add("цена близко к верхней границе бюджета"); }
					else { res.Score += 30; reasons.Add("цена в бюджете 1000–3000"); }
				}
				else warnings.Add($"цена {product.Price} руб. выше бюджета");
			}
			else if (profile.Budget <= 10000)
			{
				if (product.Price <= 3000) { res.Score += 20; reasons.Add("доступный вариант"); }
				else if (product.Price <= 10000) { res.Score += 30; reasons.Add("средний сегмент"); }
				else warnings.Add($"цена {product.Price} руб. выше среднего сегмента");
			}
			else if (profile.Budget <= 25000)
			{
				if (product.Price <= 10000) { res.Score += 15; reasons.Add("доступный вариант"); }
				else if (product.Price <= 25000) { res.Score += 30; reasons.Add("премиум-сегмент"); }
				else warnings.Add($"цена {product.Price} руб. выше бюджета");
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

		private void CheckProblems(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (string.IsNullOrEmpty(profile.Problems) || string.IsNullOrEmpty(product.Problem)) return;

			var userProblems = profile.Problems.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();

			foreach (var problem in userProblems)
			{
				if (problem == product.Problem)
				{
					res.Score += 30;
					reasons.Add($"решает «{problem}»");
					if (userProblems.Count == 1)
					{
						res.Score += 10;
						reasons.Add("решает вашу главную проблему");
					}
				}
				else if (ProblemRelations.Relations.ContainsKey(problem) && ProblemRelations.Relations[problem].Contains(product.Problem))
				{ res.Score += 15; reasons.Add($"связано с «{problem}»"); }
			}

			bool solvesAnyProblem = userProblems.Any(p => p == product.Problem ||
				(ProblemRelations.Relations.ContainsKey(p) && ProblemRelations.Relations[p].Contains(product.Problem)));

			var baseSubCategories = new List<string> {
				"Увлажнение", "Тоники и лосьоны", "Сыворотки и эссенции",
				"Кремы для лица", "Очищение (гели, пенки)", "Маски для лица",
				"Уход за глазами", "Защита от солнца (SPF)"
			};

			if (!solvesAnyProblem && userProblems.Any() && !baseSubCategories.Contains(product.SubCategory))
			{
				warnings.Add($"не решает ваши проблемы («{string.Join(", ", userProblems)}»)");
			}
		}

		private void CheckAllergies(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (string.IsNullOrEmpty(profile.Allergies) || profile.Allergies == "Нет") return;

			var userAllergies = profile.Allergies.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()).ToList();

			var allergenMap = new Dictionary<string, Func<Product, bool>>
			{
				{ "Парабены", p => p.HasParabens },
				{ "Отдушки (ароматизаторы)", p => p.HasFragrance },
				{ "Спирт (алкоголь)", p => p.HasAlcohol },
				{ "Силиконы", p => p.HasSilicones },
				{ "Сульфаты (SLS/SLES)", p => p.HasSulfates },
				{ "Глютен", p => p.HasGluten },
				{ "Орехи и их производные", p => p.HasNuts },
				{ "Эфирные масла", p => p.HasEssentialOils }
			};

			foreach (var allergy in userAllergies)
			{
				if (allergenMap.ContainsKey(allergy) && allergenMap[allergy](product))
				{
					res.Score -= 50;
					warnings.Add($"содержит «{allergy}» — аллергия!");
				}
				else
				{
					res.Score += 10;
					reasons.Add($"без «{allergy}»");
				}
			}
		}

		private void CheckSeason(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (string.IsNullOrEmpty(profile.Season) || string.IsNullOrEmpty(product.Season)) return;

			if (product.Season == profile.Season) { res.Score += 15; reasons.Add($"сезон «{profile.Season}»"); }
			else if (product.Season == "Круглый год") { res.Score += 12; reasons.Add("подходит для всех сезонов"); }
			else if (profile.Season == "Круглый год") { res.Score += 8; reasons.Add($"подходит для «{product.Season}»"); }
			else
			{
				res.Score -= 10;
				warnings.Add($"средство для «{product.Season}», а у вас «{profile.Season}»");
			}
		}

		private void CheckBrands(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (string.IsNullOrEmpty(profile.FavoriteBrands) || string.IsNullOrEmpty(product.Brand)) return;

			var userBrands = profile.FavoriteBrands.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(b => b.Trim().ToLower()).ToList();
			var productBrand = product.Brand.ToLower();

			if (userBrands.Any(b => productBrand.Contains(b) || b.Contains(productBrand)))
			{ res.Score += 25; reasons.Add($"бренд «{product.Brand}»"); }
		}

		private void CheckGoal(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (string.IsNullOrEmpty(profile.Goal)) return;

			var userGoals = profile.Goal.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(g => g.Trim()).ToList();
			var pName = product.Name.ToLower();
			var pDesc = (product.Description ?? "").ToLower();

			var goalRules = new Dictionary<string, Func<bool>>
			{
				{ "Увлажнение", () => pName.Contains("увлажн") || pDesc.Contains("увлажн") || product.SubCategory == "Увлажнение" },
				{ "Питание", () => pName.Contains("питат") || pDesc.Contains("питат") },
				{ "Антивозрастной", () => product.SubCategory == "Антивозрастной уход" },
				{ "Очищение", () => product.SubCategory == "Очищение (гели, пенки)" },
				{ "Защита", () => pName.Contains("spf") || product.SubCategory == "Защита от солнца (SPF)" },
				{ "Восстановление", () => product.SubCategory == "Сыворотки и эссенции" },
				{ "Матирование", () => product.Problem == "Расширенные поры" },
				{ "Лифтинг (подтяжка)", () => product.Problem == "Морщины" },
				{ "Осветление пигментации", () => product.Problem == "Пигментация" },
				{ "Сужение пор", () => product.Problem == "Расширенные поры" },
				{ "Сияние / здоровый вид", () => product.Problem == "Тусклый цвет" },
				{ "Снятие стресса и успокоение", () => product.Problem == "Гиперчувствительность" }
			};

			foreach (var goal in userGoals)
			{
				if (goalRules.ContainsKey(goal) && goalRules[goal]())
				{
					res.Score += 20;
					reasons.Add(goal.ToLower());
				}
			}
		}

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

		private void CheckDiet(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (profile.DietType == "Часто ем сладкое или жирное" && product.Problem == "Акне")
			{ res.Score += 15; reasons.Add("при высыпаниях из-за питания"); }
			else if (profile.DietType == "Вегетарианство" &&
				(product.Category == "Натуральная косметика" ||
				 (product.Description ?? "").ToLower().Contains("веган")))
			{ res.Score += 12; reasons.Add("для вегетарианцев"); }
		}

		private void CheckSunSensitivity(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (!profile.SunSensitivity) return;

			if (product.Name.ToLower().Contains("spf") || product.SubCategory == "Защита от солнца (SPF)")
			{ res.Score += 20; reasons.Add("SPF-защита"); }
			else
				warnings.Add("нет SPF — используйте отдельный солнцезащитный крем");
		}

		private void CheckEdema(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (!profile.TendencyToEdema) return;

			if (product.Problem == "Отечность") { res.Score += 20; reasons.Add("борется с отёками"); }
			if (product.SubCategory == "Уход за глазами") { res.Score += 10; reasons.Add("от отёков под глазами"); }
		}

		private void CheckProfessionalCare(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (!profile.HasProfessionalCare) return;

			if (product.SubCategory == "Скрабы и пилинги")
			{ res.Score -= 10; warnings.Add("скрабы могут быть избыточны"); }
			if (product.Problem == "Гиперчувствительность") { res.Score += 10; reasons.Add("после проф. ухода"); }
		}

		private void CheckTexture(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings)
		{
			if (string.IsNullOrEmpty(profile.TexturePreference) || string.IsNullOrEmpty(product.TexturePreference)) return;

			if (profile.TexturePreference == product.TexturePreference)
			{ res.Score += 10; reasons.Add($"текстура «{profile.TexturePreference}»"); }
		}

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

		private void CheckCategory(Profile profile, Product product, RecommendationResult res, List<string> reasons, List<string> warnings, bool userWantsFaceCare)
		{
			if (string.IsNullOrEmpty(product.Category)) return;

			var extraSubCategories = new List<string> { "Маски для лица", "Уход за глазами", "Скрабы и пилинги" };

			if (product.Category == "Уход за лицом" && userWantsFaceCare)
			{
				if (!extraSubCategories.Contains(product.SubCategory))
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

		private void CheckBonuses(Product product, RecommendationResult res, List<string> reasons)
		{
			if (product.IsNew) { res.Score += 5; reasons.Add("новинка"); }
			if (product.IsOnSale) { res.Score += 5; reasons.Add("акция"); }
			if (product.Rating >= 4.5) { res.Score += 5; reasons.Add($"рейтинг {product.Rating}"); }
		}
	}
}