using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LuxDustApp.Data;
using LuxDustApp.Models;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;

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

			return View(BuildViewModel(new Product()));
		}

		[HttpPost]
		public IActionResult Create(AdminProductViewModel model)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Account");

			if (string.IsNullOrWhiteSpace(model.Product.Name) ||
				string.IsNullOrWhiteSpace(model.Product.Brand) ||
				string.IsNullOrWhiteSpace(model.Product.Category) ||
				model.Product.Price <= 0)
			{
				ModelState.AddModelError("", "Заполните все обязательные поля: Название, Бренд, Категория, Цена");
				return View(BuildViewModel(model.Product));
			}

			if (string.IsNullOrEmpty(model.Product.SkinType)) model.Product.SkinType = "Нормальная";
			if (string.IsNullOrEmpty(model.Product.Problem)) model.Product.Problem = "Тусклый цвет";
			if (string.IsNullOrEmpty(model.Product.Season)) model.Product.Season = "Круглый год";
			if (string.IsNullOrEmpty(model.Product.Description)) model.Product.Description = "";
			if (string.IsNullOrEmpty(model.Product.SubCategory)) model.Product.SubCategory = "";
			if (string.IsNullOrEmpty(model.Product.TexturePreference)) model.Product.TexturePreference = "Лёгкая";
			model.Product.CreatedAt = DateTime.UtcNow;
			model.Product.Rating = 4.5;

			if (model.ImageFile != null && model.ImageFile.Length > 0)
			{
				var imageUrl = SaveImage(model.ImageFile);
				model.Product.ImageUrl = imageUrl;
			}

			if (string.IsNullOrEmpty(model.Product.ImageUrl))
				model.Product.ImageUrl = "/images/default.jpg";

			_context.Products.Add(model.Product);
			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		[HttpGet]
		public IActionResult Edit(int id)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Account");

			var product = _context.Products.FirstOrDefault(p => p.Id == id);
			if (product == null) return NotFound();

			return View(BuildViewModel(product));
		}

		[HttpPost]
		public IActionResult Edit(AdminProductViewModel model)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Account");

			if (string.IsNullOrWhiteSpace(model.Product.Name) ||
				string.IsNullOrWhiteSpace(model.Product.Brand) ||
				string.IsNullOrWhiteSpace(model.Product.Category) ||
				model.Product.Price <= 0)
			{
				ModelState.AddModelError("", "Заполните все обязательные поля: Название, Бренд, Категория, Цена");
				return View(BuildViewModel(model.Product));
			}

			var existing = _context.Products.FirstOrDefault(p => p.Id == model.Product.Id);
			if (existing == null) return NotFound();

			existing.Name = model.Product.Name;
			existing.Brand = model.Product.Brand;
			existing.Category = model.Product.Category;
			existing.SubCategory = model.Product.SubCategory;
			existing.Price = model.Product.Price;
			existing.SkinType = model.Product.SkinType;
			existing.Problem = model.Product.Problem;
			existing.Season = model.Product.Season;
			existing.Description = model.Product.Description ?? "";
			existing.IsNew = model.Product.IsNew;
			existing.IsOnSale = model.Product.IsOnSale;

			if (model.RemoveImage)
			{
				existing.ImageUrl = "/images/default.jpg";
			}

			if (model.ImageFile != null && model.ImageFile.Length > 0)
			{
				var imageUrl = SaveImage(model.ImageFile);
				existing.ImageUrl = imageUrl;
			}

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

		private string SaveImage(IFormFile file)
		{
			var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");

			if (!Directory.Exists(uploadsFolder))
				Directory.CreateDirectory(uploadsFolder);

			var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
			var filePath = Path.Combine(uploadsFolder, uniqueName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				file.CopyTo(stream);
			}

			return "/images/products/" + uniqueName;
		}

		private AdminProductViewModel BuildViewModel(Product product)
		{
			return new AdminProductViewModel
			{
				Product = product,
				Brands = _context.Products.Where(p => p.Brand != null && p.Brand != "").Select(p => p.Brand).Distinct().OrderBy(b => b).ToList(),
				Categories = _context.Products.Where(p => p.Category != null && p.Category != "").Select(p => p.Category).Distinct().OrderBy(c => c).ToList(),
				SubcategoriesByCategory = GetSubcategoriesByCategory(),
				SkinTypes = new List<string> { "Сухая", "Жирная", "Комбинированная", "Нормальная", "Чувствительная", "Обезвоженная", "Склонная к куперозу" },
				Problems = new List<string> { "Акне", "Пигментация", "Морщины", "Купероз", "Тусклый цвет", "Гиперчувствительность", "Чёрные точки", "Сухость", "Расширенные поры", "Отечность" },
				Seasons = new List<string> { "Лето", "Зима", "Демисезон", "Круглый год" }
			};
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