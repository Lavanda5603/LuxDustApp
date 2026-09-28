// ApplicationDbContext.cs - Контекст базы данных для Entity Framework Core

using Microsoft.EntityFrameworkCore;
using LuxDustApp.Models;

namespace LuxDustApp.Data
{
	public class ApplicationDbContext : DbContext
	{
		// Конструктор принимает настройки подключения к PostgreSQL
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		// Пользователи системы
		public DbSet<User> Users { get; set; }

		// Анкеты пользователей
		public DbSet<Profile> Profiles { get; set; }

		// Косметические продукты
		public DbSet<Product> Products { get; set; }

		// История подборок
		public DbSet<Recommendation> Recommendations { get; set; }

		// Избранные товары
		public DbSet<Favorite> Favorites { get; set; }

		// Корзина покупок
		public DbSet<Cart> Carts { get; set; }

		// Промокоды
		public DbSet<PromoCode> PromoCodes { get; set; }

		// Подарочные карты
		public DbSet<GiftCard> GiftCards { get; set; }

		// Связь заказов и подарочных карт
		public DbSet<OrderGiftCard> OrderGiftCards { get; set; }

		// Заказы
		public DbSet<Order> Orders { get; set; }

		// Товары в заказе
		public DbSet<OrderItem> OrderItems { get; set; }
	}
}