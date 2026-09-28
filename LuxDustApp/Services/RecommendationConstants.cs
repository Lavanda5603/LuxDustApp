// RecommendationConstants.cs - Словари и константы для алгоритма подбора

using LuxDustApp.Models;
using System;
using System.Collections.Generic;

namespace LuxDustApp.Services
{
	public static class RecommendationConstants
	{
		// Категории, которые я исключаю из подбора (не относятся к уходу за лицом)
		public static readonly List<string> ExcludedCategories = new List<string>
		{
			"Подборки", "Уборка и стирка", "Хобби и творчество", "Фигура мечты"
		};

		// Категории, которые я исключаю, если пользователь хочет уход за лицом
		public static readonly List<string> NonFaceCareCategories = new List<string>
		{
			"Уход за телом", "Волосы", "Макияж", "Для мужчин", "Для детей",
			"Здоровье и БАДы", "Парфюмерия", "Для дома", "Аксессуары",
			"Мини-форматы", "Маникюр и педикюр"
		};

		// Базовые подкатегории ухода за лицом (которые не считаются лишними)
		public static readonly List<string> BaseFaceSubCategories = new List<string>
		{
			"Увлажнение", "Тоники и лосьоны", "Сыворотки и эссенции",
			"Кремы для лица", "Очищение (гели, пенки)", "Маски для лица",
			"Уход за глазами", "Защита от солнца (SPF)"
		};

		// Связи между проблемами
		public static readonly Dictionary<string, List<string>> ProblemRelations = new Dictionary<string, List<string>>
		{
			{ "Акне", new List<string> { "Чёрные точки", "Расширенные поры", "Неровный тон кожи" } },
			{ "Чёрные точки", new List<string> { "Акне", "Расширенные поры" } },
			{ "Расширенные поры", new List<string> { "Акне", "Чёрные точки" } },
			{ "Сухость/Шелушение", new List<string> { "Гиперчувствительность" } },
			{ "Гиперчувствительность", new List<string> { "Сухость/Шелушение", "Купероз" } },
			{ "Купероз", new List<string> { "Гиперчувствительность" } },
			{ "Морщины", new List<string> { "Тусклый цвет", "Неровный тон кожи" } },
			{ "Тусклый цвет", new List<string> { "Морщины", "Неровный тон кожи" } },
			{ "Пигментация", new List<string> { "Неровный тон кожи" } },
			{ "Неровный тон кожи", new List<string> { "Пигментация", "Тусклый цвет" } },
			{ "Отечность", new List<string> { "Расширенные поры" } }
		};

		// Словарь аллергенов
		public static readonly Dictionary<string, Func<Product, bool>> AllergenMap = new Dictionary<string, Func<Product, bool>>
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

		// Словарь целей ухода
		public static readonly Dictionary<string, Func<Product, bool>> GoalRules = new Dictionary<string, Func<Product, bool>>
		{
			{ "Увлажнение", p => (p.Name ?? "").ToLower().Contains("увлажн") || (p.Description ?? "").ToLower().Contains("увлажн") || p.SubCategory == "Увлажнение" },
			{ "Питание", p => (p.Name ?? "").ToLower().Contains("питат") || (p.Description ?? "").ToLower().Contains("питат") },
			{ "Антивозрастной", p => p.SubCategory == "Антивозрастной уход" },
			{ "Очищение", p => p.SubCategory == "Очищение (гели, пенки)" },
			{ "Защита", p => (p.Name ?? "").ToLower().Contains("spf") || p.SubCategory == "Защита от солнца (SPF)" },
			{ "Восстановление", p => p.SubCategory == "Сыворотки и эссенции" },
			{ "Матирование", p => p.Problem == "Расширенные поры" },
			{ "Лифтинг (подтяжка)", p => p.Problem == "Морщины" },
			{ "Осветление пигментации", p => p.Problem == "Пигментация" },
			{ "Сужение пор", p => p.Problem == "Расширенные поры" },
			{ "Сияние / здоровый вид", p => p.Problem == "Тусклый цвет" },
			{ "Снятие стресса и успокоение", p => p.Problem == "Гиперчувствительность" }
		};

		// Цели, которые означают, что пользователь хочет уход за лицом
		public static readonly List<string> FaceCareGoals = new List<string>
		{
			"Увлажнение", "Очищение", "Антивозрастной", "Питание",
			"Сияние", "Матирование", "Сужение пор", "Лифтинг",
			"Осветление", "Восстановление", "Снятие стресса"
		};
	}
}