using LuxDustApp.Data;
using LuxDustApp.Models;
using LuxDustApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace LuxDustApp.Controllers
{
	public class GiftCardsController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly GiftCardService _giftCardService;

		public GiftCardsController(ApplicationDbContext context, GiftCardService giftCardService)
		{
			_context = context;
			_giftCardService = giftCardService;
		}

		private int? GetUserId()
		{
			var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			return claim != null ? int.Parse(claim) : null;
		}

		[HttpGet]
		public IActionResult Index()
		{
			if (GetUserId() == null)
				return RedirectToAction("Login", "Account");

			return View();
		}

		[HttpPost]
		public IActionResult Buy(int amount)
		{
			var userId = GetUserId();
			if (userId == null)
				return RedirectToAction("Login", "Account");

			if (amount != 1000 && amount != 3000 && amount != 5000)
				return BadRequest();

			var card = new GiftCard
			{
				Code = _giftCardService.GenerateCode(),
				Amount = amount,
				RemainingAmount = amount,
				IsActive = true,
				OwnerUserId = userId.Value,
				CreatedAt = DateTime.UtcNow,
				ExpiresAt = DateTime.UtcNow.AddYears(1)
			};

			_context.GiftCards.Add(card);
			_context.SaveChanges();

			return RedirectToAction("Success", new { id = card.Id });
		}

		[HttpGet]
		public IActionResult Success(int id)
		{
			var userId = GetUserId();
			if (userId == null)
				return RedirectToAction("Login", "Account");

			var card = _context.GiftCards.FirstOrDefault(g => g.Id == id && g.OwnerUserId == userId);
			if (card == null) return NotFound();

			return View(card);
		}

		[HttpGet]
		public IActionResult MyCards()
		{
			var userId = GetUserId();
			if (userId == null)
				return RedirectToAction("Login", "Account");

			var cards = _context.GiftCards
				.Where(g => g.OwnerUserId == userId).OrderByDescending(g => g.CreatedAt).ToList();

			return View(cards);
		}
	}
}