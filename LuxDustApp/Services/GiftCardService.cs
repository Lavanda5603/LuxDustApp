using System;

namespace LuxDustApp.Services
{
	public class GiftCardService
	{
		private static readonly Random _random = new Random();

		public string GenerateCode()
		{
			return $"LUX-{Part()}-{Part()}-{Part()}";
		}

		private string Part()
		{
			return _random.Next(0x1000, 0xFFFF).ToString("X4");
		}
	}
}