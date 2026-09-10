using Microsoft.AspNetCore.Mvc;
using LuxDustApp.Data;
using LuxDustApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LuxDustApp.Controllers
{
	public class AccountController : Controller
	{
		private readonly ApplicationDbContext _context;

		public AccountController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Profile()
		{
			var userId = 1;

			var profile = _context.Profiles
				.FirstOrDefault(p => p.UserId == userId);

			var recommendations = _context.Recommendations.Where(r => r.UserId == userId).Include(r => r.Product)
				.OrderByDescending(r => r.RecommendedAt).Take(10).ToList();

			var favorites = _context.Favorites.Where(f => f.UserId == userId).Include(f => f.Product).ToList();

			ViewBag.Profile = profile;
			ViewBag.Recommendations = recommendations;
			ViewBag.Favorites = favorites;

			return View();
		}
	}
}