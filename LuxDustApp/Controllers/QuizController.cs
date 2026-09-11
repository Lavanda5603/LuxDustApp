using LuxDustApp.Data;
using LuxDustApp.Models;
using LuxDustApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace LuxDustApp.Controllers
{
	public class QuizController : Controller
	{
		private readonly RecommendationService _recommendationService;
		private readonly ApplicationDbContext _context;

		public QuizController(RecommendationService recommendationService, ApplicationDbContext context)
		{
			_recommendationService = recommendationService;
			_context = context;
		}

		public IActionResult Step1()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Results(Profile profile)
		{
			var productsWithScores = _recommendationService.GetRecommendationsWithScores(profile);

			var userId = 1;

			foreach (var kvp in productsWithScores)
			{
				var recommendation = new Recommendation
				{
					UserId = userId,
					ProductId = kvp.Key.Id,
					Score = kvp.Value,
					RecommendedAt = System.DateTime.UtcNow
				};
				_context.Recommendations.Add(recommendation);
			}
			_context.SaveChanges();

			ViewBag.UserSkinType = profile.SkinType;
			ViewBag.UserBudget = profile.Budget;
			ViewBag.UserProblem = profile.Problems;
			ViewBag.UserAllergies = profile.Allergies;
			ViewBag.UserSeason = profile.Season;
			ViewBag.UserGoal = profile.Goal;

			return View(productsWithScores.Keys.ToList());
		}

		public IActionResult AddToFavorites(int productId)
		{
			var userId = 1;

			var existing = _context.Favorites.FirstOrDefault(f => f.UserId == userId && f.ProductId == productId);

			if (existing == null)
			{
				var favorite = new Favorite
				{
					UserId = userId,
					ProductId = productId,
					AddedAt = System.DateTime.UtcNow
				};
				_context.Favorites.Add(favorite);
				_context.SaveChanges();
			}

			return RedirectToAction("Profile", "Account");
		}

		public IActionResult AddToCart(int productId)
		{
			var userId = 1;

			var existing = _context.Carts.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

			if (existing != null)
			{
				existing.Quantity += 1;
			}
			else
			{
				var cart = new Cart
				{
					UserId = userId,
					ProductId = productId,
					Quantity = 1,
					AddedAt = System.DateTime.UtcNow
				};
				_context.Carts.Add(cart);
			}

			_context.SaveChanges();
			return RedirectToAction("Cart", "Quiz");
		}

		public IActionResult Cart()
		{
			var userId = 1;

			var cartItems = _context.Carts.Where(c => c.UserId == userId).Include(c => c.Product).ToList();

			return View(cartItems);
		}

		public IActionResult RemoveFromCart(int cartId)
		{
			var item = _context.Carts.FirstOrDefault(c => c.Id == cartId);
			if (item != null)
			{
				_context.Carts.Remove(item);
				_context.SaveChanges();
			}

			return RedirectToAction("Cart", "Quiz");
		}
	}
}