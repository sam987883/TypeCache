// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Data.Common;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TypeCache.Attributes;
using TypeCache.Data;
using TypeCache.Mediation;
using TypeCache.Net.Mediation;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Registers singleton <c><see cref="IDataSource"/></c> to be available via: <c>IAccessor&lt;<see cref="IDataSource"/>&gt;</c><br/>
	/// </summary>
	public static IServiceCollection AddDataSource(this IServiceCollection @this, string name, DbProviderFactory dbProviderFactory, string connectionString, DataSourceType type)
		=> @this.AddSingleton<IDataSource>(new DataSource(name, dbProviderFactory, connectionString, type));

	/// <summary>
	/// Provides data source related information: <c>IAccessor&lt;<see cref="IDataSource"/>&gt;</c><br/>
	/// <i><b>Requires call to:</b></i>
	/// <code><see cref="DbProviderFactories.RegisterFactory(string, DbProviderFactory)"/></code>
	/// </summary>
	public static IServiceCollection AddDataSourceAccessor(this IServiceCollection @this)
		=> @this.AddSingleton<IAccessor<IDataSource>, Accessor<IDataSource>>();

	/// <summary>
	/// Registers Singletons:
	/// <list type="bullet">
	/// <item><term><c><see cref="IHashMaker"/></c></term> Utility class that encrypts a long to a simple string hashed ID and back.</item>
	/// </list>
	/// </summary>
	/// <param name="rgbKey">Any random 16 bytes</param>
	/// <param name="rgbIV">Any random 16 bytes</param>
	public static IServiceCollection AddHashMaker(this IServiceCollection @this, byte[] rgbKey, byte[] rgbIV)
		=> @this.AddSingleton<IHashMaker>(provider => new HashMaker(rgbKey, rgbIV));

	/// <summary>
	/// Registers Singletons:
	/// <list type="bullet">
	/// <item><term><c><see cref="IHashMaker"/></c></term> Utility class that encrypts a long to a simple string hashed ID and back.</item>
	/// </list>
	/// </summary>
	/// <param name="rgbKey">Any random decimal value (gets converted to a 16 byte array)</param>
	/// <param name="rgbIV">Any random decimal value (gets converted to a 16 byte array)</param>
	public static IServiceCollection AddHashMaker(this IServiceCollection @this, decimal rgbKey, decimal rgbIV)
		=> @this.AddSingleton<IHashMaker>(provider => new HashMaker(rgbKey.ToBytes(), rgbIV.ToBytes()));

	/// <summary>
	/// Registers Singletons:
	/// <list type="bullet">
	/// <item><term><c><see cref="IHashMaker"/></c></term> Utility class that encrypts a long to a simple string hashed ID and back.</item>
	/// </list>
	/// </summary>
	/// <param name="rgbKey">Any random 8 characters</param>
	/// <param name="rgbIV">Any random 8 characters</param>
	public static IServiceCollection AddHashMaker(this IServiceCollection @this, ReadOnlySpan<char> rgbKey, ReadOnlySpan<char> rgbIV)
		=> @this.AddHashMaker(rgbKey.AsBytes().ToArray(), rgbIV.AsBytes().ToArray());

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.AddSingleton&lt;IRule&lt;<see cref="HttpClientRequest"/>, <see cref="HttpResponseMessage"/>&gt;, <see cref="HttpClientRule"/>&gt;()</c>
	/// </summary>
	/// <remarks>Requires call to: <c>IServiceCollection.AddHttpClient</c></remarks>
	public static IServiceCollection AddHttpClientRule(this IServiceCollection @this)
		=> @this.AddSingleton<IRule<HttpClientRequest, HttpResponseMessage>, HttpClientRule>();

	/// <summary>
	/// Registers Singleton: <c><see cref="IMediator"/></c><br/>
	/// Optionally can register Rules and After Rules.
	/// </summary>
	public static IServiceCollection AddMediation(this IServiceCollection @this, Action<RulesBuilder>? rulesBuilder = null)
	{
		@this.AddSingleton<IMediator, Mediator>();
		rulesBuilder?.Invoke(new RulesBuilder(@this));
		return @this;
	}

	/// <summary>
	/// Registers all types in the specified assembly that have <see cref="ServiceLifetimeAttribute{T}"/> or <see cref="ServiceLifetimeAttribute{T}"/>.
	/// </summary>
	/// <param name="fromAssembly">The assembly to register the types from.</param>
	public static IServiceCollection AddTypes(this IServiceCollection @this, Assembly fromAssembly)
	{
		var serviceDescriptors = fromAssembly.GetTypes()
			.Where(type => !type.IsAbstract && !type.IsGenericType && !type.IsInterface && !type.IsPointer && !type.IsPrimitive
				&& type.HasCustomAttribute<ServiceLifetimeAttribute>())
			.Select(implementationType =>
			{
				var attribute = implementationType.GetCustomAttribute<ServiceLifetimeAttribute>()!;
				return new ServiceDescriptor(attribute.ServiceType ?? implementationType, implementationType, attribute.ServiceLifetime);
			})
			.ToArray();
		serviceDescriptors.ForEach(@this.Add);

		return @this;
	}
}
