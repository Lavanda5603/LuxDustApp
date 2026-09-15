using System.Collections.Generic;

namespace LuxDustApp.Services
{
	public static class ProblemRelations
	{
		public static readonly Dictionary<string, List<string>> Relations = new Dictionary<string, List<string>>
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
	}
}