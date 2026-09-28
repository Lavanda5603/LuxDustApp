// RecommendationResult.cs - Результат одного товара (балл + причины)

namespace LuxDustApp.Models
{
	public class RecommendationResult
	{
		// Итоговый балл совместимости
		public int Score { get; set; }

		// Строка с причинами («✓ тип кожи «Сухая»; бюджетный вариант»)
		public string Reasons { get; set; } = "";
	}
}