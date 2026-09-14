using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LuxDustApp.Data;
using LuxDustApp.Models;
using System.Linq;
using System.Security.Claims;

namespace LuxDustApp.Controllers
{
	public class AdminController : Controller
	{
		private readonly ApplicationDbContext _context;

		public AdminController(ApplicationDbContext context)
		{
			_context = context;
		}

		private bool IsAdmin()
		{
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userIdClaim == null) return false;

			var userId = int.Parse(userIdClaim);
			var user = _context.Users.FirstOrDefault(u => u.Id == userId);
			return user != null && user.IsAdmin;
		}

		public IActionResult Index()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Account");

			var products = _context.Products.ToList();
			ViewBag.TotalProducts = products.Count;
			ViewBag.TotalUsers = _context.Users.Count();
			ViewBag.TotalOrders = 0;

			return View(products);
		}

		public IActionResult Create()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Account");

			var model = new AdminProductViewModel
			{
				Product = new Product(),
				Brands = _context.Products.Where(p => p.Brand != null && p.Brand != "").Select(p => p.Brand).Distinct().OrderBy(b => b).ToList(),
				Categories = _context.Products.Where(p => p.Category != null && p.Category != "").Select(p => p.Category).Distinct().OrderBy(c => c).ToList(),
				SubcategoriesByCategory = GetSubcategoriesByCategory(),
				SkinTypes = new List<string> { "Сухая", "Жирная", "Комбинированная", "Нормальная", "Чувствительная", "Обезвоженная", "Склонная к куперозу" },
				Problems = new List<string> { "Акне", "Пигментация", "Морщины", "Купероз", "Тусклый цвет", "Гиперчувствительность", "Чёрные точки", "Сухость", "Расширенные поры", "Отечность" },
				Seasons = new List<string> { "Лето", "Зима", "Демисезон", "Круглый год" }
			};

			return View(model);
		}


		[HttpPost]
		public IActionResult Create(Product product)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Account");

			if (string.IsNullOrWhiteSpace(product.Name) ||
				string.IsNullOrWhiteSpace(product.Brand) ||
				string.IsNullOrWhiteSpace(product.Category) ||
				product.Price <= 0)
			{
				ModelState.AddModelError("", "Заполните все обязательные поля: Название, Бренд, Категория, Цена");

				var model = new AdminProductViewModel
				{
					Product = product,
					Brands = _context.Products.Where(p => p.Brand != null && p.Brand != "").Select(p => p.Brand).Distinct().OrderBy(b => b).ToList(),
					Categories = _context.Products.Where(p => p.Category != null && p.Category != "").Select(p => p.Category).Distinct().OrderBy(c => c).ToList(),
					SubcategoriesByCategory = GetSubcategoriesByCategory(),
					SkinTypes = new List<string> { "Сухая", "Жирная", "Комбинированная", "Нормальная", "Чувствительная", "Обезвоженная", "Склонная к куперозу" },
					Problems = new List<string> { "Акне", "Пигментация", "Морщины", "Купероз", "Тусклый цвет", "Гиперчувствительность", "Чёрные точки", "Сухость", "Расширенные поры", "Отечность" },
					Seasons = new List<string> { "Лето", "Зима", "Демисезон", "Круглый год" }
				};
				return View(model);
			}

			if (string.IsNullOrEmpty(product.SkinType)) product.SkinType = "Нормальная";
			if (string.IsNullOrEmpty(product.Problem)) product.Problem = "Тусклый цвет";
			if (string.IsNullOrEmpty(product.Season)) product.Season = "Круглый год";
			if (string.IsNullOrEmpty(product.Description)) product.Description = "";
			if (string.IsNullOrEmpty(product.ImageUrl)) product.ImageUrl = "/images/default.jpg";
			if (string.IsNullOrEmpty(product.SubCategory)) product.SubCategory = "";
			if (string.IsNullOrEmpty(product.TexturePreference)) product.TexturePreference = "Лёгкая";
			product.CreatedAt = System.DateTime.UtcNow;
			product.Rating = 4.5;

			_context.Products.Add(product);
			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult Edit(Product product)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Account");

			if (string.IsNullOrWhiteSpace(product.Name) ||
				string.IsNullOrWhiteSpace(product.Brand) ||
				string.IsNullOrWhiteSpace(product.Category) ||
				product.Price <= 0)
			{
				ModelState.AddModelError("", "Заполните все обязательные поля: Название, Бренд, Категория, Цена");

				var model = new AdminProductViewModel
				{
					Product = product,
					Brands = _context.Products.Where(p => p.Brand != null && p.Brand != "").Select(p => p.Brand).Distinct().OrderBy(b => b).ToList(),
					Categories = _context.Products.Where(p => p.Category != null && p.Category != "").Select(p => p.Category).Distinct().OrderBy(c => c).ToList(),
					SubcategoriesByCategory = GetSubcategoriesByCategory(),
					SkinTypes = new List<string> { "Сухая", "Жирная", "Комбинированная", "Нормальная", "Чувствительная", "Обезвоженная", "Склонная к куперозу" },
					Problems = new List<string> { "Акне", "Пигментация", "Морщины", "Купероз", "Тусклый цвет", "Гиперчувствительность", "Чёрные точки", "Сухость", "Расширенные поры", "Отечность" },
					Seasons = new List<string> { "Лето", "Зима", "Демисезон", "Круглый год" }
				};
				return View(model);
			}

			var existing = _context.Products.FirstOrDefault(p => p.Id == product.Id);
			if (existing == null) return NotFound();

			existing.Name = product.Name;
			existing.Brand = product.Brand;
			existing.Category = product.Category;
			existing.SubCategory = product.SubCategory;
			existing.Price = product.Price;
			existing.SkinType = product.SkinType;
			existing.Problem = product.Problem;
			existing.Season = product.Season;
			existing.Description = product.Description ?? "";
			existing.IsNew = product.IsNew;
			existing.IsOnSale = product.IsOnSale;

			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		public IActionResult Delete(int id)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Account");

			var product = _context.Products.FirstOrDefault(p => p.Id == id);
			if (product != null)
			{
				_context.Products.Remove(product);
				_context.SaveChanges();
			}

			return RedirectToAction("Index");
		}

		private Dictionary<string, List<string>> GetSubcategoriesByCategory()
		{
			var result = new Dictionary<string, List<string>>();
			var categories = _context.Products.Where(p => p.Category != null && p.Category != "").Select(p => p.Category).Distinct().ToList();

			foreach (var cat in categories)
			{
				var subs = _context.Products
					.Where(p => p.Category == cat && p.SubCategory != null && p.SubCategory != "")
					.Select(p => p.SubCategory).Distinct().OrderBy(s => s).ToList();
				result[cat] = subs;
			}

			return result;
		}
	}
}