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

namespace LuxDustApp.Controllers
{
	public class AccountController : Controller
	{
		private readonly ApplicationDbContext _context;

		public AccountController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Profile()
		{
			var userId = GetUserId();
			if (userId == null) return RedirectToAction("Login");

			var profile = _context.Profiles.FirstOrDefault(p => p.UserId == userId);
			var recommendations = _context.Recommendations.Where(r => r.UserId == userId).Include(r => r.Product)
				.OrderByDescending(r => r.RecommendedAt).Take(10).ToList();
			var favorites = _context.Favorites.Where(f => f.UserId == userId).Include(f => f.Product).ToList();

			ViewBag.Profile = profile;
			ViewBag.Recommendations = recommendations;
			ViewBag.Favorites = favorites;
			ViewBag.UserName = User.Identity.Name;

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
			if (!ModelState.IsValid) return View(model);

			if (_context.Users.Any(u => u.Email == model.Email))
			{
				ModelState.AddModelError("Email", "Пользователь с таким email уже существует");
				return View(model);
			}

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

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, user.Name),
				new Claim(ClaimTypes.Email, user.Email)
			};

			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

			return RedirectToAction("Profile");
		}

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Home");
		}

		private int? GetUserId()
		{
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			return userIdClaim != null ? int.Parse(userIdClaim) : null;
		}
	}
}