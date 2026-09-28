// LoginViewModel.cs - Модель для входа

using System.ComponentModel.DataAnnotations;

namespace LuxDustApp.Models
{
	public class LoginViewModel
	{
		// Email обязателен и должен быть валидным
		[Required(ErrorMessage = "Введите email")]
		[EmailAddress(ErrorMessage = "Некорректный email")]
		public string Email { get; set; } = null!;

		// Пароль обязателен
		[Required(ErrorMessage = "Введите пароль")]
		public string Password { get; set; } = null!;
	}
}