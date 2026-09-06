using Microsoft.AspNetCore.Mvc;

namespace LuxDustApp.Controllers
{
	public class QuizController : Controller
	{
		public IActionResult Step1()
		{
			return View();
		}
	}
}