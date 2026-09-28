// CatalogController.cs - Каталог товаров, поиск и фильтрация

using Microsoft.AspNetCore.Mvc;
using LuxDustApp.Data;
using LuxDustApp.Models;
using System.Linq;
using System.Collections.Generic;
using System;

namespace LuxDustApp.Controllers
{
	public class CatalogController : Controller
	{
		private readonly ApplicationDbContext _context;

		public CatalogController(ApplicationDbContext context)
		{
			_context = context;
		}

		// Параметры nullable — если не переданы, значение = null
		public IActionResult Index(string? category = null, string? subcategory = null, string? search = null, int? minPrice = null, int? maxPrice = null, string? filter = null, int page = 1)
		{
			var products = _context.Products.AsQueryable();

			// Цены не могут быть отрицательными
			if (minPrice.HasValue && minPrice.Value < 0) minPrice = 0;
			if (maxPrice.HasValue && maxPrice.Value < 0) maxPrice = 0;

			// Если minPrice > maxPrice, меняю их местами
			if (minPrice.HasValue && maxPrice.HasValue && minPrice.Value > maxPrice.Value)
			{
				var temp = minPrice;
				minPrice = maxPrice;
				maxPrice = temp;
			}

			// Фильтрую по категории
			if (!string.IsNullOrEmpty(category))
				products = products.Where(p => p.Category == category);

			// Фильтрую по подкатегории
			if (!string.IsNullOrEmpty(subcategory))
				products = products.Where(p => p.SubCategory == subcategory);

			// Фильтрую по поиску
			if (!string.IsNullOrEmpty(search))
				products = products.Where(p => (p.Name ?? "").ToLower().Contains(search.ToLower()));

			// Фильтрую по цене
			if (minPrice.HasValue)
				products = products.Where(p => p.Price >= minPrice.Value);

			if (maxPrice.HasValue)
				products = products.Where(p => p.Price <= maxPrice.Value);

			// Фильтрую по акции/новинке
			if (filter == "sale")
				products = products.Where(p => p.IsOnSale);

			if (filter == "new")
				products = products.Where(p => p.IsNew);

			// Пагинация: 12 товаров на страницу
			int pageSize = 12;
			int totalItems = products.Count();
			int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

			if (page < 1) page = 1;
			if (page > totalPages && totalPages > 0) page = totalPages;

			var pagedProducts = products.OrderBy(p => p.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();

			// Формирую список категорий и подкатегорий
			var categories = _context.Products
				.Where(p => p.Category != null && p.Category != "")
				.Select(p => p.Category!)
				.Distinct()
				.OrderBy(c => c)
				.ToList();

			var subcategoriesByCategory = new Dictionary<string, List<string>>();
			foreach (var cat in categories)
			{
				// Пропускаю пустые категории
				if (string.IsNullOrEmpty(cat)) continue;

				subcategoriesByCategory[cat] = _context.Products
					.Where(p => p.Category == cat && p.SubCategory != null && p.SubCategory != "")
					.Select(p => p.SubCategory!)
					.Distinct()
					.OrderBy(s => s)
					.ToList();
			}

			ViewBag.Categories = categories;
			ViewBag.SubcategoriesByCategory = subcategoriesByCategory;
			ViewBag.CurrentCategory = category;
			ViewBag.CurrentSubcategory = subcategory;
			ViewBag.CurrentSearch = search;
			ViewBag.MinPrice = minPrice;
			ViewBag.MaxPrice = maxPrice;
			ViewBag.Filter = filter;
			ViewBag.CurrentPage = page;
			ViewBag.TotalPages = totalPages;
			ViewBag.TotalItems = totalItems;

			return View(pagedProducts);
		}

		public IActionResult Details(int id)
		{
			var product = _context.Products.FirstOrDefault(p => p.Id == id);
			if (product == null) return NotFound();
			return View(product);
		}

		// Параметры nullable
		[HttpGet]
		public IActionResult Search(string? query = null, int? minPrice = null, int? maxPrice = null, string? category = null, string? subcategory = null)
		{
			var products = _context.Products.AsQueryable();

			// Валидация цен
			if (minPrice.HasValue && minPrice.Value < 0) minPrice = 0;
			if (maxPrice.HasValue && maxPrice.Value < 0) maxPrice = 0;

			if (!string.IsNullOrEmpty(category))
				products = products.Where(p => p.Category == category);

			if (!string.IsNullOrEmpty(subcategory))
				products = products.Where(p => p.SubCategory == subcategory);

			if (!string.IsNullOrEmpty(query))
				products = products.Where(p => (p.Name ?? "").ToLower().Contains(query.ToLower()));

			if (minPrice.HasValue)
				products = products.Where(p => p.Price >= minPrice.Value);

			if (maxPrice.HasValue)
				products = products.Where(p => p.Price <= maxPrice.Value);

			return PartialView("_ProductCards", products.Take(12).ToList());
		}
	}
}