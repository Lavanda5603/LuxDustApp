using System.ComponentModel.DataAnnotations;

namespace LuxDustApp.Models
{
	public class User
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Введите email")]
		[EmailAddress(ErrorMessage = "Некорректный email")]
		public string Email { get; set; }

		[Required]
		public string PasswordHash { get; set; }

		[Required(ErrorMessage = "Введите имя")]
		public string Name { get; set; }

		public bool IsAdmin { get; set; } = false;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public string? AvatarUrl { get; set; }
		public string? FullName { get; set; }
		public DateTime? BirthDate { get; set; }
		public string? City { get; set; }
	}
}