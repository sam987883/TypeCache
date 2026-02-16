// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public static class ServiceCollectionExtensions
{
	extension(IServiceCollection @this)
	{
		/// <summary>
		/// Registers singletons:
		/// <list type="bullet">
		/// <item><term><c><see cref="IHashMaker"/></c></term> Utility class that encrypts a <see cref="long"/> to a simple string hashed ID and back.</item>
		/// </list>
		/// </summary>
		/// <param name="rgbKey">Any random 16 bytes</param>
		public IServiceCollection AddHashMaker(byte[] rgbKey)
			=> @this.AddSingleton<IHashMaker>(provider => new HashMaker(rgbKey));

		/// <summary>
		/// Registers singletons:
		/// <list type="bullet">
		/// <item><term><c><see cref="IHashMaker"/></c></term> Utility class that encrypts a <see cref="long"/> to a simple string hashed ID and back.</item>
		/// </list>
		/// </summary>
		/// <param name="rgbKey">Any random 16 bytes</param>
		/// <param name="rgbIV">Any random 16 bytes</param>
		public IServiceCollection AddHashMaker(byte[] rgbKey, byte[] rgbIV)
			=> @this.AddSingleton<IHashMaker>(provider => new HashMaker(rgbKey, rgbIV));

		/// <summary>
		/// Registers singletons:
		/// <list type="bullet">
		/// <item><term><c><see cref="IHashMaker"/></c></term> Utility class that encrypts a <see cref="long"/> to a simple string hashed ID and back.</item>
		/// </list>
		/// </summary>
		/// <param name="rgbKey">Any random 16 UTF-8 characters</param>
		public IServiceCollection AddHashMaker(ReadOnlySpan<char> rgbKey)
			=> @this.AddHashMaker(rgbKey.AsBytes().ToArray());

		/// <summary>
		/// Registers singletons:
		/// <list type="bullet">
		/// <item><term><c><see cref="IHashMaker"/></c></term> Utility class that encrypts a <see cref="long"/> to a simple string hashed ID and back.</item>
		/// </list>
		/// </summary>
		/// <param name="rgbKey">Any random 16 UTF-8 characters</param>
		/// <param name="rgbIV">Any random 16 UTF-8 characters</param>
		public IServiceCollection AddHashMaker(ReadOnlySpan<char> rgbKey, ReadOnlySpan<char> rgbIV)
			=> @this.AddHashMaker(rgbKey.AsBytes().ToArray(), rgbIV.AsBytes().ToArray());

		public IServiceCollection AddServiceDescriptor<SERVICE>(ServiceLifetime serviceLifetime, Func<IServiceProvider, SERVICE> factory)
			where SERVICE : class
		{
			@this.Add(ServiceDescriptor.Describe(typeof(SERVICE), factory, serviceLifetime));
			return @this;
		}

		public IServiceCollection AddServiceDescriptor<SERVICE, IMPLEMENTATION>(ServiceLifetime serviceLifetime)
			where SERVICE : class
		{
			@this.Add(ServiceDescriptor.Describe(typeof(SERVICE), typeof(IMPLEMENTATION), serviceLifetime));
			return @this;
		}

		public IServiceCollection AddServiceDescriptor<SERVICE>(object? key, ServiceLifetime serviceLifetime, Func<IServiceProvider, object?, SERVICE> factory)
			where SERVICE : class
		{
			@this.Add(ServiceDescriptor.DescribeKeyed(typeof(SERVICE), key, factory, serviceLifetime));
			return @this;
		}

		public IServiceCollection AddServiceDescriptor<SERVICE, IMPLEMENTATION>(ServiceLifetime serviceLifetime, object? key = null)
			where SERVICE : class
			where IMPLEMENTATION : class
		{
			@this.Add(ServiceDescriptor.DescribeKeyed(typeof(SERVICE), key, typeof(IMPLEMENTATION), serviceLifetime));
			return @this;
		}
	}
}
