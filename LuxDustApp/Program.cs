// Program.cs - Точка входа в приложение LuxDustApp

using LuxDustApp.Data;
using LuxDustApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Подключаю MVC
builder.Services.AddControllersWithViews();

// Подключаю базу данных PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрирую мои сервисы (DI)
builder.Services.AddScoped<RecommendationService>();
builder.Services.AddScoped<GiftCardService>();

// Настраиваю cookie-аутентификацию
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/Account/Login";   // куда отправлять неавторизованных
		options.LogoutPath = "/Account/Logout"; // куда отправлять при выходе
		options.ExpireTimeSpan = TimeSpan.FromDays(7); // сессия живёт 7 дней
	});

var app = builder.Build();

// Обработка ошибок в продакшене
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

// Перенаправление на HTTPS
app.UseHttpsRedirection();

// Раздача статики (CSS, JS, картинки)
app.UseStaticFiles();

// Маршрутизация
app.UseRouting();

// Аутентификация и авторизация
app.UseAuthentication();
app.UseAuthorization();

// Дефолтный маршрут: Home/Index
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();