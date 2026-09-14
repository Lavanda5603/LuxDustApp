using LuxDustApp.Data;
using LuxDustApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;

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
		public IActionResult PlaceOrder(string deliveryMethod, string address, DateTime deliveryDate, string deliveryTime, string phone, string comment)
		{
			var userId = GetUserId();
			if (userId == null) return RedirectToAction("Login", "Account");

			var cartItems = _context.Carts.Where(c => c.UserId == userId).Include(c => c.Product).ToList();
			if (!cartItems.Any()) return RedirectToAction("Cart", "Quiz");

			int totalPrice = cartItems.Sum(c => c.Product.Price * c.Quantity);
			int deliveryPrice = deliveryMethod == "Курьер" ? 300 : 0;
			
			var order = new Order
			{
				UserId = userId.Value,
				DeliveryMethod = deliveryMethod ?? "",
				Address = address ?? "",
				DeliveryDate = deliveryDate.ToUniversalTime(),
				DeliveryTime = deliveryTime ?? "",
				Phone = phone ?? "",
				Comment = comment ?? "",
				TotalPrice = totalPrice + deliveryPrice,
				DeliveryPrice = deliveryPrice,
				Discount = 0,
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
	}
}