// Copyright(c) 2020 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;

namespace TypeCache.Security.Extensions
{
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Registers Singletons:
		/// <list type="bullet">
		/// <item><term><see cref="IHashMaker"/></term> <description>Utility class that encrypts a long to a simple string hashed ID and back.</description></item>
		/// </list>
		/// </summary>
		/// <param name="rgbKey">Any random 16 bytes</param>
		/// <param name="rgbIV">Any random 16 bytes</param>
		public static IServiceCollection RegisterHashMaker(this IServiceCollection @this, byte[] rgbKey, byte[] rgbIV)
			=> @this.AddSingleton<IHashMaker>(new HashMaker(rgbKey, rgbIV));

		/// <summary>
		/// Registers Singletons:
		/// <list type="bullet">
		/// <item><term><see cref="IHashMaker"/></term> <description>Utility class that encrypts a long to a simple string hashed ID and back.</description></item>
		/// </list>
		/// </summary>
		/// <param name="rgbKey">Any random decimal value (gets converted to a 16 byte array)</param>
		/// <param name="rgbIV">Any random decimal value (gets converted to a 16 byte array)</param>
		public static IServiceCollection RegisterHashMaker(this IServiceCollection @this, decimal rgbKey, decimal rgbIV)
			=> @this.AddSingleton<IHashMaker>(new HashMaker(rgbKey, rgbIV));
	}
}
