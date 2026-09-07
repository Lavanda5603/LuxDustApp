using Microsoft.AspNetCore.Mvc;
using LuxDustApp.Services;

namespace LuxDustApp.Controllers
{
	public class QuizController : Controller
	{
		private readonly RecommendationService _recommendationService;

		public QuizController()
		{
			_recommendationService = new RecommendationService();
		}

		public IActionResult Step1()
		{
			return View();
		}

		public IActionResult Results()
		{
			var products = _recommendationService.GetRecommendations();

			return View(products);
		}
	}
}