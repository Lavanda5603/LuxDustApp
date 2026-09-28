// RegisterViewModel.cs - Модель для регистрации

using System.ComponentModel.DataAnnotations;

namespace LuxDustApp.Models
{
	public class RegisterViewModel
	{
		// Имя
		[Required(ErrorMessage = "Введите имя")]
		public string Name { get; set; } = null!;

		// Email (проверяю на валидность)
		[Required(ErrorMessage = "Введите email")]
		[EmailAddress(ErrorMessage = "Некорректный email")]
		public string Email { get; set; } = null!;

		// Пароль (минимум 6 символов, буквы + цифры + спецсимвол)
		[Required(ErrorMessage = "Введите пароль")]
		[MinLength(6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
		[RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[!@#$%^&*]).+$",
			ErrorMessage = "Пароль должен содержать буквы, цифры и спецсимвол (!@#$%^&*)")]
		public string Password { get; set; } = null!;

		// Подтверждение пароля
		[Required(ErrorMessage = "Подтвердите пароль")]
		[Compare("Password", ErrorMessage = "Пароли не совпадают")]
		public string ConfirmPassword { get; set; } = null!;
	}
}