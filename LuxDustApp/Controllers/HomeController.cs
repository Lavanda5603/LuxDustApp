// HomeController.cs - Главная страница: акции и новинки

using Microsoft.AspNetCore.Mvc;
using LuxDustApp.Data;
using System.Linq;

namespace LuxDustApp.Controllers
{
	public class HomeController : Controller
	{
		private readonly ApplicationDbContext _context;

		public HomeController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			// Загружаю последние 10 товаров со скидкой
			ViewBag.SaleProducts = _context.Products
				.Where(p => p.IsOnSale)
				.OrderByDescending(p => p.Id)
				.Take(10)
				.ToList();

			// Загружаю последние 10 новинок
			ViewBag.NewProducts = _context.Products
				.Where(p => p.IsNew)
				.OrderByDescending(p => p.Id)
				.Take(10)
				.ToList();

			return View();
		}
	}
}