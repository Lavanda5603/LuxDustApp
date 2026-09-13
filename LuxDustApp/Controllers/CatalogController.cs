using Microsoft.AspNetCore.Mvc;
using LuxDustApp.Data;
using LuxDustApp.Models;
using System.Linq;
using System.Collections.Generic;

namespace LuxDustApp.Controllers
{
	public class CatalogController : Controller
	{
		private readonly ApplicationDbContext _context;

		public CatalogController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Index(string category = null, string subcategory = null)
		{
			var products = _context.Products.AsQueryable();

			if (!string.IsNullOrEmpty(category))
				products = products.Where(p => p.Category == category);

			if (!string.IsNullOrEmpty(subcategory))
				products = products.Where(p => p.SubCategory == subcategory);

			var categories = _context.Products
				.Where(p => p.Category != null && p.Category != "").Select(p => p.Category).Distinct().ToList();

			var subcategories = new List<string>();
			if (!string.IsNullOrEmpty(category))
			{
				subcategories = _context.Products
					.Where(p => p.Category == category && p.SubCategory != null && p.SubCategory != "")
					.Select(p => p.SubCategory).Distinct().ToList();
			}

			ViewBag.Categories = categories;
			ViewBag.Subcategories = subcategories;
			ViewBag.CurrentCategory = category;
			ViewBag.CurrentSubcategory = subcategory;

			return View(products.ToList());
		}

		public IActionResult Details(int id)
		{
			var product = _context.Products.FirstOrDefault(p => p.Id == id);
			if (product == null) return NotFound();
			return View(product);
		}
	}
}