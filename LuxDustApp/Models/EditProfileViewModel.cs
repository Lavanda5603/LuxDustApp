using Microsoft.AspNetCore.Http;
using System;

namespace LuxDustApp.Models
{
	public class EditProfileViewModel
	{
		public string? FullName { get; set; }
		public DateTime? BirthDate { get; set; }
		public string? City { get; set; }
		public IFormFile? AvatarFile { get; set; }
		public string? CurrentAvatarUrl { get; set; }
		public bool RemoveAvatar { get; set; }
	}
}