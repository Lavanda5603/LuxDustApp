using Microsoft.AspNetCore.Mvc;
using LuxDustApp.Services;
using LuxDustApp.Models;

namespace LuxDustApp.Controllers
{
	public class QuizController : Controller
	{
		private readonly RecommendationService _recommendationService;

		public QuizController(RecommendationService recommendationService)
		{
			_recommendationService = recommendationService;
		}

		public IActionResult Step1()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Results(Profile profile)
		{
			var products = _recommendationService.GetRecommendations(profile);
			return View(products);
		}
	}
}