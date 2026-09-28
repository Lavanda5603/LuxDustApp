// EditProfileViewModel.cs - Модель для редактирования профиля

using Microsoft.AspNetCore.Http;
using System;

namespace LuxDustApp.Models
{
	public class EditProfileViewModel
	{
		// ФИО пользователя
		public string? FullName { get; set; }

		// Дата рождения
		public DateTime? BirthDate { get; set; }

		// Город
		public string? City { get; set; }

		// Загружаемый файл аватара
		public IFormFile? AvatarFile { get; set; }

		// Текущий URL аватара
		public string? CurrentAvatarUrl { get; set; }

		// Галочка «Удалить текущий аватар»
		public bool RemoveAvatar { get; set; }
	}
}