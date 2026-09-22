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

			ViewBag.AllBrands = _context.Products
				.Where(p => p.Brand != null && p.Brand != "").Select(p => p.Brand!).Distinct().OrderBy(b => b).ToList();

			ViewBag.AllCategories = _context.Products
				.Where(p => p.Category != null && p.Category != "").Select(p => p.Category!).Distinct().OrderBy(c => c).ToList();

			return View(products);
		}

		[HttpGet]
		public IActionResult Search(string query = null, string brand = null, string category = null, string filter = null)
		{
			if (!IsAdmin()) return Unauthorized();

			var products = _context.Products.AsQueryable();

			if (!string.IsNullOrEmpty(query))
				products = products.Where(p => p.Name.ToLower().Contains(query.ToLower()));

			if (!string.IsNullOrEmpty(brand))
				products = products.Where(p => p.Brand == brand);

			if (!string.IsNullOrEmpty(category))
				products = products.Where(p => p.Category == category);

			if (filter == "new")
				products = products.Where(p => p.IsNew);

			if (filter == "sale")
				products = products.Where(p => p.IsOnSale);

			var list = products.OrderBy(p => p.Id).ToList();
			return PartialView("_AdminProductRows", list);
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

			var product = model.Product ?? new Product();

			if (string.IsNullOrEmpty(product.Name)) product.Name = "Без названия";
			if (string.IsNullOrEmpty(product.Brand)) product.Brand = "Без бренда";
			if (string.IsNullOrEmpty(product.Category)) product.Category = "Без категории";
			if (string.IsNullOrEmpty(product.SubCategory)) product.SubCategory = "";
			if (string.IsNullOrEmpty(product.SkinType)) product.SkinType = "Нормальная";
			if (string.IsNullOrEmpty(product.Problem)) product.Problem = "Тусклый цвет";
			if (string.IsNullOrEmpty(product.Season)) product.Season = "Круглый год";
			if (string.IsNullOrEmpty(product.Description)) product.Description = "";
			if (string.IsNullOrEmpty(product.TexturePreference)) product.TexturePreference = "Лёгкая";
			if (string.IsNullOrEmpty(product.ImageUrl)) product.ImageUrl = "/images/default.jpg";

			product.CreatedAt = DateTime.UtcNow;
			product.Rating = 4.5;

			if (model.ImageFile != null && model.ImageFile.Length > 0)
			{
				product.ImageUrl = SaveImage(model.ImageFile);
			}

			_context.Products.Add(product);
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

			var productData = model.Product ?? new Product();

			var existing = _context.Products.FirstOrDefault(p => p.Id == productData.Id);
			if (existing == null) return NotFound();

			if (!string.IsNullOrEmpty(productData.Name)) existing.Name = productData.Name;
			if (!string.IsNullOrEmpty(productData.Brand)) existing.Brand = productData.Brand;
			if (!string.IsNullOrEmpty(productData.Category)) existing.Category = productData.Category;
			if (!string.IsNullOrEmpty(productData.SubCategory)) existing.SubCategory = productData.SubCategory;
			if (productData.Price > 0) existing.Price = productData.Price;
			if (!string.IsNullOrEmpty(productData.SkinType)) existing.SkinType = productData.SkinType;
			if (!string.IsNullOrEmpty(productData.Problem)) existing.Problem = productData.Problem;
			if (!string.IsNullOrEmpty(productData.Season)) existing.Season = productData.Season;
			if (!string.IsNullOrEmpty(productData.Description)) existing.Description = productData.Description;
			existing.IsNew = productData.IsNew;
			existing.IsOnSale = productData.IsOnSale;

			if (model.RemoveImage)
			{
				existing.ImageUrl = "/images/default.jpg";
			}

			if (model.ImageFile != null && model.ImageFile.Length > 0)
			{
				existing.ImageUrl = SaveImage(model.ImageFile);
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