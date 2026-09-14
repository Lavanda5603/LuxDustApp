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

		public IActionResult Index(string category = null, string subcategory = null, string search = null, int? minPrice = null, int? maxPrice = null, int page = 1)
		{
			var products = _context.Products.AsQueryable();

			if (!string.IsNullOrEmpty(category))
				products = products.Where(p => p.Category == category);

			if (!string.IsNullOrEmpty(subcategory))
				products = products.Where(p => p.SubCategory == subcategory);

			if (!string.IsNullOrEmpty(search))
				products = products.Where(p => p.Name.ToLower().Contains(search.ToLower()));

			if (minPrice.HasValue)
				products = products.Where(p => p.Price >= minPrice.Value);
			if (maxPrice.HasValue)
				products = products.Where(p => p.Price <= maxPrice.Value);

			int pageSize = 12;
			int totalItems = products.Count();
			int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

			if (page < 1) page = 1;
			if (page > totalPages && totalPages > 0) page = totalPages;

			var pagedProducts = products
				.OrderBy(p => p.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();

			var categories = _context.Products
				.Where(p => p.Category != null && p.Category != "").Select(p => p.Category).Distinct().OrderBy(c => c).ToList();

			var subcategoriesByCategory = new Dictionary<string, List<string>>();
			foreach (var cat in categories)
			{
				var subs = _context.Products
					.Where(p => p.Category == cat && p.SubCategory != null && p.SubCategory != "")
					.Select(p => p.SubCategory).Distinct().OrderBy(s => s).ToList();
				subcategoriesByCategory[cat] = subs;
			}

			ViewBag.Categories = categories;
			ViewBag.SubcategoriesByCategory = subcategoriesByCategory;
			ViewBag.CurrentCategory = category;
			ViewBag.CurrentSubcategory = subcategory;
			ViewBag.CurrentSearch = search;
			ViewBag.MinPrice = minPrice;
			ViewBag.MaxPrice = maxPrice;
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

		[HttpGet]
		public IActionResult Search(string query = null, int? minPrice = null, int? maxPrice = null, string category = null, string subcategory = null)
		{
			var products = _context.Products.AsQueryable();

			if (!string.IsNullOrEmpty(category))
				products = products.Where(p => p.Category == category);

			if (!string.IsNullOrEmpty(subcategory))
				products = products.Where(p => p.SubCategory == subcategory);

			if (!string.IsNullOrEmpty(query))
				products = products.Where(p => p.Name.ToLower().Contains(query.ToLower()));

			if (minPrice.HasValue)
				products = products.Where(p => p.Price >= minPrice.Value);
			if (maxPrice.HasValue)
				products = products.Where(p => p.Price <= maxPrice.Value);

			var result = products.Take(12).ToList();

			return PartialView("_ProductCards", result);
		}
	}
}