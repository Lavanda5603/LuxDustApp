// QuizController.cs - Управление анкетой, подбором, корзиной и избранным

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

		// Вспомогательный метод для получения ID текущего пользователя
		private int? GetUserId()
		{
			var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			return userIdClaim != null ? int.Parse(userIdClaim) : null;
		}

		public IActionResult Step1()
		{
			// Если пользователь не авторизован, отправляю его на регистрацию
			if (User.Identity?.IsAuthenticated != true)
			{
				return RedirectToAction("Register", "Account");
			}
			return View();
		}

		[HttpPost]
		public IActionResult Results(Profile profile)
		{
			// Проверяю, авторизован ли пользователь
			var userId = GetUserId();
			if (userId == null) return RedirectToAction("Login", "Account");

			// Если ключевые поля пустые, возвращаю на анкету с ошибкой
			if (string.IsNullOrEmpty(profile.SkinType) || profile.Age <= 0 || profile.Budget <= 0)
			{
				ModelState.AddModelError("", "Пожалуйста, заполните все обязательные поля анкеты.");
				return View("Step1", profile);
			}

			// Запускаю алгоритм подбора и получаю подборку с причинами
			var bundle = _recommendationService.GetRecommendationsWithReasons(profile);

			// Сохраняю каждую рекомендацию в историю
			foreach (var kvp in bundle.TopReasons)
			{
				_context.Recommendations.Add(new Recommendation
				{
					UserId = userId.Value,
					ProductId = kvp.Key.Id,
					Score = kvp.Value.Score,
					Reasons = kvp.Value.Reasons,
					RecommendedAt = System.DateTime.UtcNow
				});
			}
			_context.SaveChanges();

			// Проверяю, есть ли у пользователя уже анкета
			var existingProfile = _context.Profiles.FirstOrDefault(p => p.UserId == userId.Value);
			if (existingProfile == null)
			{
				// Если анкеты нет, создаю новую
				profile.UserId = userId.Value;
				profile.UpdatedAt = System.DateTime.UtcNow;
				_context.Profiles.Add(profile);
			}
			else
			{
				// Если анкета есть, обновляю все поля
				existingProfile.SkinType = profile.SkinType;
				existingProfile.Age = profile.Age;
				existingProfile.Budget = profile.Budget;
				existingProfile.Problems = profile.Problems;
				existingProfile.Allergies = profile.Allergies;
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

			return View(bundle);
		}

		public IActionResult AddToFavorites(int productId)
		{
			var userId = GetUserId();
			if (userId == null) return Json(new { success = false, message = "Не авторизован" });

			// Проверяю, нет ли уже этого товара в избранном
			var existing = _context.Favorites.FirstOrDefault(f => f.UserId == userId.Value && f.ProductId == productId);

			if (existing == null)
			{
				_context.Favorites.Add(new Favorite
				{
					UserId = userId.Value,
					ProductId = productId,
					AddedAt = System.DateTime.UtcNow
				});
				_context.SaveChanges();
			}

			return Json(new { success = true });
		}

		public IActionResult AddToCart(int productId)
		{
			var userId = GetUserId();
			if (userId == null) return Json(new { success = false, message = "Не авторизован" });

			// Проверяю, есть ли уже этот товар в корзине
			var existing = _context.Carts.FirstOrDefault(c => c.UserId == userId.Value && c.ProductId == productId);

			if (existing != null)
			{
				// Если есть, увеличиваю количество
				existing.Quantity += 1;
			}
			else
			{
				// Если нет, добавляю новый товар
				_context.Carts.Add(new Cart
				{
					UserId = userId.Value,
					ProductId = productId,
					Quantity = 1,
					AddedAt = System.DateTime.UtcNow
				});
			}
			_context.SaveChanges();

			return Json(new { success = true });
		}

		public IActionResult Cart()
		{
			var userId = GetUserId();
			if (userId == null) return RedirectToAction("Login", "Account");

			// Загружаю все товары в корзине пользователя
			var cartItems = _context.Carts.Where(c => c.UserId == userId.Value).Include(c => c.Product).ToList();
			return View(cartItems);
		}

		public IActionResult RemoveFromCart(int cartId)
		{
			// Нахожу товар в корзине и удаляю его
			var item = _context.Carts.FirstOrDefault(c => c.Id == cartId);
			if (item != null)
			{
				_context.Carts.Remove(item);
				_context.SaveChanges();
			}
			return RedirectToAction("Cart", "Quiz");
		}

		[HttpPost]
		public IActionResult UpdateQuantity(int cartId, int delta)
		{
			var userId = GetUserId();
			if (userId == null) return Json(new { success = false });

			// Нахожу товар в корзине текущего пользователя
			var item = _context.Carts.FirstOrDefault(c => c.Id == cartId && c.UserId == userId.Value);
			if (item == null) return Json(new { success = false });

			// Обновляю количество с ограничениями (от 1 до 99)
			item.Quantity += delta;
			if (item.Quantity < 1) item.Quantity = 1;
			if (item.Quantity > 99) item.Quantity = 99;

			_context.SaveChanges();

			return Json(new { success = true, quantity = item.Quantity });
		}
	}
}