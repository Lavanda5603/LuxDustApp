using LuxDustApp.Data;
using LuxDustApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;

namespace LuxDustApp.Controllers
{
	public class CheckoutController : Controller
	{
		private readonly ApplicationDbContext _context;

		public CheckoutController(ApplicationDbContext context)
		{
			_context = context;
		}

		private int? GetUserId()
		{
			var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			return claim != null ? int.Parse(claim) : null;
		}

		public IActionResult Index()
		{
			var userId = GetUserId();
			if (userId == null) return RedirectToAction("Login", "Account");

			var cartItems = _context.Carts.Where(c => c.UserId == userId).Include(c => c.Product).ToList();
			if (!cartItems.Any()) return RedirectToAction("Cart", "Quiz");

			return View(cartItems);
		}

		[HttpPost]
		public IActionResult PlaceOrder(
			string deliveryMethod, string address, DateTime deliveryDate,
			string deliveryTime, string phone, string comment,
			string? appliedPromoCode, int appliedDiscount,
			string? appliedGiftCards)
		{
			var userId = GetUserId();
			if (userId == null) return RedirectToAction("Login", "Account");

			var cartItems = _context.Carts.Where(c => c.UserId == userId).Include(c => c.Product).ToList();
			if (!cartItems.Any()) return RedirectToAction("Cart", "Quiz");

			int totalPrice = cartItems.Sum(c => c.Product.Price * c.Quantity);
			int deliveryPrice = deliveryMethod == "Курьер" ? 300 : 0;

			int promoDiscount = 0;
			string? promoCode = null;

			if (!string.IsNullOrEmpty(appliedPromoCode) && appliedDiscount > 0)
			{
				var promo = _context.PromoCodes.FirstOrDefault(p => p.Code == appliedPromoCode && p.IsActive);
				if (promo != null)
				{
					promoDiscount = appliedDiscount;
					promoCode = promo.Code;
					promo.TimesUsed += 1;
				}
			}

			int giftCardTotal = 0;
			var giftCardEntries = new List<(GiftCard card, int amount)>();

			if (!string.IsNullOrEmpty(appliedGiftCards))
			{
				var codes = appliedGiftCards.Split(',', StringSplitOptions.RemoveEmptyEntries);
				int alreadyApplied = 0;

				foreach (var rawCode in codes)
				{
					var code = rawCode.Trim().ToUpper();
					var card = _context.GiftCards.FirstOrDefault(g => g.Code == code && g.IsActive);
					if (card == null || card.RemainingAmount <= 0) continue;

					int remainingOrderAmount = totalPrice + deliveryPrice - promoDiscount - alreadyApplied;
					if (remainingOrderAmount <= 0) break;

					int amountToApply = Math.Min(card.RemainingAmount, remainingOrderAmount);
					alreadyApplied += amountToApply;
					giftCardTotal += amountToApply;
					giftCardEntries.Add((card, amountToApply));
				}
			}

			int finalTotal = totalPrice + deliveryPrice - promoDiscount - giftCardTotal;
			if (finalTotal < 0) finalTotal = 0;

			var order = new Order
			{
				UserId = userId.Value,
				DeliveryMethod = deliveryMethod ?? "",
				Address = address ?? "",
				DeliveryDate = deliveryDate.ToUniversalTime(),
				DeliveryTime = deliveryTime ?? "",
				Phone = phone ?? "",
				Comment = comment ?? "",
				TotalPrice = finalTotal,
				DeliveryPrice = deliveryPrice,
				Discount = promoDiscount + giftCardTotal,
				PromoCode = promoCode,
				PromoDiscount = promoDiscount,
				CreatedAt = DateTime.UtcNow,
				Status = "В обработке"
			};

			_context.Orders.Add(order);
			_context.SaveChanges();

			foreach (var item in cartItems)
			{
				_context.OrderItems.Add(new OrderItem
				{
					OrderId = order.Id,
					ProductId = item.ProductId,
					Quantity = item.Quantity,
					Price = item.Product.Price
				});
			}
			_context.SaveChanges();

			foreach (var (card, amount) in giftCardEntries)
			{
				card.RemainingAmount -= amount;
				if (card.RemainingAmount <= 0)
				{
					card.RemainingAmount = 0;
					card.IsActive = false;
					card.UsedAt = DateTime.UtcNow;
					card.UsedByUserId = userId.Value;
				}

				_context.OrderGiftCards.Add(new OrderGiftCard
				{
					OrderId = order.Id,
					GiftCardId = card.Id,
					AppliedAmount = amount
				});
			}
			_context.SaveChanges();

			_context.Carts.RemoveRange(cartItems);
			_context.SaveChanges();

			return RedirectToAction("Success", new { id = order.Id });
		}

		public IActionResult Success(int id)
		{
			var order = _context.Orders.FirstOrDefault(o => o.Id == id);
			if (order == null) return NotFound();

			var today = DateTime.UtcNow.Date;
			var deliveryDate = order.DeliveryDate.Date;

			if (order.DeliveryMethod == "Самовывоз")
			{
				if (deliveryDate < today)
					order.Status = "Получен";
				else if (deliveryDate == today)
					order.Status = "Можно забирать";
				else
					order.Status = "Готовится к выдаче";
			}
			else
			{
				if (deliveryDate < today)
					order.Status = "Доставлен";
				else if (deliveryDate == today)
					order.Status = "Курьер в пути";
				else
					order.Status = "В обработке";
			}

			_context.SaveChanges();

			return View(order);
		}

		[HttpPost]
		public IActionResult ApplyPromo(string code, int cartTotal)
		{
			if (string.IsNullOrWhiteSpace(code))
				return Json(new { success = false, message = "Введите промокод" });

			code = code.Trim().ToUpper();

			var promo = _context.PromoCodes.FirstOrDefault(p => p.Code == code);

			if (promo == null)
				return Json(new { success = false, message = "Промокод не найден" });

			if (!promo.IsActive)
				return Json(new { success = false, message = "Промокод неактивен" });

			if (promo.ExpiresAt.HasValue && promo.ExpiresAt.Value < DateTime.UtcNow)
				return Json(new { success = false, message = "Промокод истёк" });

			if (promo.MaxUses > 0 && promo.TimesUsed >= promo.MaxUses)
				return Json(new { success = false, message = "Лимит использований исчерпан" });

			if (cartTotal < promo.MinOrderAmount)
				return Json(new { success = false, message = $"Минимальная сумма заказа {promo.MinOrderAmount} руб." });

			int discount = 0;
			if (promo.DiscountPercent > 0)
				discount = cartTotal * promo.DiscountPercent / 100;
			else if (promo.FixedDiscount > 0)
				discount = promo.FixedDiscount;

			if (discount > cartTotal)
				discount = cartTotal;

			return Json(new
			{
				success = true,
				code = promo.Code,
				discount = discount,
				message = $"Промокод применён: −{discount} руб."
			});
		}

		[HttpPost]
		public IActionResult ApplyGiftCard(string code, int cartTotal, int promoDiscount, int alreadyApplied)
		{
			if (string.IsNullOrWhiteSpace(code))
				return Json(new { success = false, message = "Введите код карты" });

			code = code.Trim().ToUpper();

			var card = _context.GiftCards.FirstOrDefault(g => g.Code == code);

			if (card == null)
				return Json(new { success = false, message = "Карта не найдена" });

			if (!card.IsActive)
				return Json(new { success = false, message = "Карта неактивна" });

			if (card.ExpiresAt.HasValue && card.ExpiresAt.Value < DateTime.UtcNow)
				return Json(new { success = false, message = "Карта истекла" });

			if (card.RemainingAmount <= 0)
				return Json(new { success = false, message = "На карте нет средств" });

			int remainingOrderAmount = cartTotal - promoDiscount - alreadyApplied;
			if (remainingOrderAmount <= 0)
				return Json(new { success = false, message = "Сумма заказа уже покрыта" });

			int amountToApply = Math.Min(card.RemainingAmount, remainingOrderAmount);

			return Json(new
			{
				success = true,
				cardId = card.Id,
				code = card.Code,
				amount = amountToApply,
				message = $"Применено: −{amountToApply} руб."
			});
		}
	}
}