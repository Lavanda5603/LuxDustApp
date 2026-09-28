// User.cs - Модель пользователя

using System;
using System.ComponentModel.DataAnnotations;

namespace LuxDustApp.Models
{
	public class User
	{
		public int Id { get; set; }

		// Email (валидирую)
		[Required(ErrorMessage = "Введите email")]
		[EmailAddress(ErrorMessage = "Некорректный email")]
		public string Email { get; set; } = null!;

		// Хэш пароля (BCrypt)
		[Required]
		public string PasswordHash { get; set; } = null!;

		// Имя
		[Required(ErrorMessage = "Введите имя")]
		public string Name { get; set; } = null!;

		// Флаг администратора
		public bool IsAdmin { get; set; } = false;

		// Дата регистрации
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		// URL аватара
		public string? AvatarUrl { get; set; }

		// ФИО (для анкеты)
		public string? FullName { get; set; }

		// Дата рождения
		public DateTime? BirthDate { get; set; }

		// Город
		public string? City { get; set; }
	}
}