// AccountController.cs - Управление пользователями, регистрацией и профилем

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using LuxDustApp.Data;
using LuxDustApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BCrypt.Net;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace LuxDustApp.Controllers
{
	public class AccountController : Controller
	{
		private readonly ApplicationDbContext _context;

		public AccountController(ApplicationDbContext context)
		{
			_context = context;
		}

		// Вспомогательный метод для получения ID текущего пользователя
		private int? GetUserId()
		{
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			return userIdClaim != null ? int.Parse(userIdClaim) : null;
		}

		public IActionResult Profile()
		{
			var userId = GetUserId();
			if (userId == null) return RedirectToAction("Login");

			// Загружаю анкету пользователя
			var profile = _context.Profiles.FirstOrDefault(p => p.UserId == userId.Value);

			// Загружаю историю подборок (последние 10)
			var recommendations = _context.Recommendations
				.Where(r => r.UserId == userId.Value)
				.Include(r => r.Product)
				.OrderByDescending(r => r.RecommendedAt)
				.Take(10)
				.ToList();

			// Загружаю избранное
			var favorites = _context.Favorites
				.Where(f => f.UserId == userId.Value)
				.Include(f => f.Product)
				.ToList();

			// Загружаю заказы пользователя
			var orders = _context.Orders
				.Where(o => o.UserId == userId.Value)
				.Include(o => o.Items)
				.ThenInclude(oi => oi.Product)
				.OrderByDescending(o => o.CreatedAt)
				.ToList();

			// Обновляю статусы заказов в зависимости от даты доставки
			foreach (var order in orders)
			{
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
			}

			_context.SaveChanges();

			// Передаю данные в представление
			ViewBag.Profile = profile;
			ViewBag.Recommendations = recommendations;
			ViewBag.Favorites = favorites;
			ViewBag.Orders = orders;

			var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
			ViewBag.UserName = User.Identity?.Name;
			ViewBag.User = user;

			return View();
		}

		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Register(RegisterViewModel model)
		{
			// Проверяю валидацию модели (имя, email, пароль)
			if (!ModelState.IsValid) return View(model);

			// Проверяю, нет ли уже пользователя с таким email
			if (_context.Users.Any(u => u.Email == model.Email))
			{
				ModelState.AddModelError("Email", "Пользователь с таким email уже существует");
				return View(model);
			}

			// Хэширую пароль через BCrypt
			var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

			var user = new User
			{
				Email = model.Email,
				PasswordHash = passwordHash,
				Name = model.Name
			};

			_context.Users.Add(user);
			_context.SaveChanges();

			return RedirectToAction("Login");
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (!ModelState.IsValid) return View(model);

			var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
			if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
			{
				ModelState.AddModelError("", "Неверный email или пароль");
				return View(model);
			}

			// Формирую claims для cookie-аутентификации
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, user.Name),
				new Claim(ClaimTypes.Email, user.Email)
			};

			if (user.IsAdmin)
			{
				claims.Add(new Claim(ClaimTypes.Role, "Admin"));
			}

			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

			return RedirectToAction("Profile");
		}

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public IActionResult EditProfile()
		{
			var userId = GetUserId();
			if (userId == null) return RedirectToAction("Login");

			var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
			if (user == null) return NotFound();

			var model = new EditProfileViewModel
			{
				FullName = user.FullName,
				BirthDate = user.BirthDate,
				City = user.City,
				CurrentAvatarUrl = user.AvatarUrl
			};

			return View(model);
		}

		[HttpPost]
		public IActionResult EditProfile(EditProfileViewModel model)
		{
			var userId = GetUserId();
			if (userId == null) return RedirectToAction("Login");

			// Дата рождения не может быть в будущем
			if (model.BirthDate.HasValue && model.BirthDate.Value.Date > DateTime.UtcNow.Date)
			{
				ModelState.AddModelError("BirthDate", "Дата рождения не может быть в будущем");
				return View(model);
			}

			// Дата рождения не может быть слишком старой (больше 120 лет)
			if (model.BirthDate.HasValue && model.BirthDate.Value.Year < DateTime.UtcNow.Year - 120)
			{
				ModelState.AddModelError("BirthDate", "Проверьте дату рождения");
				return View(model);
			}

			// Размер файла аватара не больше 5 МБ
			if (model.AvatarFile != null && model.AvatarFile.Length > 5 * 1024 * 1024)
			{
				ModelState.AddModelError("AvatarFile", "Размер файла не должен превышать 5 МБ");
				return View(model);
			}

			// Тип файла аватара — только изображения
			if (model.AvatarFile != null && !IsImageFile(model.AvatarFile.FileName))
			{
				ModelState.AddModelError("AvatarFile", "Разрешены только изображения (JPG, JPEG, PNG)");
				return View(model);
			}

			var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
			if (user == null) return NotFound();

			user.FullName = model.FullName;
			user.BirthDate = model.BirthDate.HasValue
				? DateTime.SpecifyKind(model.BirthDate.Value, DateTimeKind.Utc)
				: null;
			user.City = model.City;

			if (model.RemoveAvatar)
			{
				user.AvatarUrl = null;
			}

			if (model.AvatarFile != null && model.AvatarFile.Length > 0)
			{
				user.AvatarUrl = SaveAvatar(model.AvatarFile);
			}

			_context.SaveChanges();

			return RedirectToAction("Profile");
		}

		// Проверяю, является ли файл изображением
		private bool IsImageFile(string fileName)
		{
			var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
			var extension = Path.GetExtension(fileName).ToLower();
			return allowedExtensions.Contains(extension);
		}

		// Сохраняю аватар в папку wwwroot/images/avatars
		private string SaveAvatar(IFormFile file)
		{
			var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "avatars");

			if (!Directory.Exists(uploadsFolder))
				Directory.CreateDirectory(uploadsFolder);

			var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
			var filePath = Path.Combine(uploadsFolder, uniqueName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				file.CopyTo(stream);
			}

			return "/images/avatars/" + uniqueName;
		}
	}
}