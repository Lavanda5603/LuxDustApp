using Microsoft.AspNetCore.Mvc;

namespace LuxDustApp.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
