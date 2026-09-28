// GiftCardService.cs - Генерация уникальных кодов подарочных карт

using System;
using System.Security.Cryptography;

namespace LuxDustApp.Services
{
	public class GiftCardService
	{
		// Генерирую код формата LUX-XXXX-XXXX-XXXX
		public string GenerateCode()
		{
			return $"LUX-{Part()}-{Part()}-{Part()}";
		}

		// Генерирую одну часть кода (4 hex-символа)
		private string Part()
		{
			// Использую криптографический генератор, чтобы коды нельзя было подобрать
			byte[] bytes = new byte[2];
			RandomNumberGenerator.Fill(bytes);
			ushort value = BitConverter.ToUInt16(bytes, 0);
			return value.ToString("X4");
		}
	}
}