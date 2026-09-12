using System.ComponentModel.DataAnnotations;

namespace LuxDustApp.Models
{
	public class RegisterViewModel
	{
		[Required(ErrorMessage = "Введите имя")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Введите email")]
		[EmailAddress(ErrorMessage = "Некорректный email")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Введите пароль")]
		[MinLength(6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
		[RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[!@#$%^&*]).+$",
			ErrorMessage = "Пароль должен содержать буквы, цифры и спецсимвол (!@#$%^&*)")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Подтвердите пароль")]
		[Compare("Password", ErrorMessage = "Пароли не совпадают")]
		public string ConfirmPassword { get; set; }
	}
}