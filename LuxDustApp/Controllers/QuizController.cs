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
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Register", "Account");
			}

			return View();
		}

		[HttpPost]
		public IActionResult Results(Profile profile)
		{
			var productsWithScores = _recommendationService.GetRecommendationsWithScores(profile);

			var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			if (userIdClaim == null) return RedirectToAction("Login", "Account");
			var userId = int.Parse(userIdClaim);

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

			var existingProfile = _context.Profiles.FirstOrDefault(p => p.UserId == userId);
			if (existingProfile == null)
			{
				profile.UserId = userId;
				profile.UpdatedAt = System.DateTime.UtcNow;
				_context.Profiles.Add(profile);
			}
			else
			{
				existingProfile.SkinType = profile.SkinType;
				existingProfile.Age = profile.Age;
				existingProfile.Budget = profile.Budget;
				existingProfile.Problems = profile.Problems;
				existingProfile.ProblemsOther = profile.ProblemsOther;
				existingProfile.Allergies = profile.Allergies;
				existingProfile.AllergiesOther = profile.AllergiesOther;
				existingProfile.Season = profile.Season;
				existingProfile.FavoriteBrands = profile.FavoriteBrands;
				existingProfile.Goal = profile.Goal;
				existingProfile.StressLevel = profile.StressLevel;
				existingProfile.DietType = profile.DietType;
				existingProfile.SunSensitivity = profile.SunSensitivity;
				existingProfile.TendencyToEdema = profile.TendencyToEdema;
				existingProfile.HasProfessionalCare = profile.HasProfessionalCare;
				existingProfile.TexturePreference = profile.TexturePreference;
				existingProfile.ReadyForMultiStep = profile.ReadyForMultiStep;
				existingProfile.UpdatedAt = System.DateTime.UtcNow;
			}
			_context.SaveChanges();

			return View(productsWithScores.Keys.ToList());
		}

		public IActionResult AddToFavorites(int productId)
		{
			var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			if (userIdClaim == null) return RedirectToAction("Login", "Account");
			var userId = int.Parse(userIdClaim);

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
			var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			if (userIdClaim == null) return Json(new { success = false, message = "Не авторизован" });
			var userId = int.Parse(userIdClaim);

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

			return Json(new { success = true });
		}

		public IActionResult Cart()
		{
			var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			if (userIdClaim == null) return RedirectToAction("Login", "Account");
			var userId = int.Parse(userIdClaim);

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