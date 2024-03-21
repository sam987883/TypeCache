// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TypeCache.Attributes;
using TypeCache.Data;
using TypeCache.Data.Mediation;
using TypeCache.Mediation;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Registers singleton: <c><see cref="IAfterRule{REQUEST}"/></c>.<br/>
	/// Requires registering rule: <c><see cref="IRule{REQUEST}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddAfterRule<REQUEST>(this IServiceCollection @this, IAfterRule<REQUEST> afterRule)
		where REQUEST : IRequest
		=> @this.AddSingleton<IAfterRule<REQUEST>>(afterRule);

	/// <summary>
	/// Registers singleton: <c><see cref="IAfterRule{REQUEST, RESPONSE}"/></c>.<br/>
	/// Requires registering rule: <c><see cref="IRule{REQUEST}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddAfterRule<REQUEST, RESPONSE>(this IServiceCollection @this, IAfterRule<REQUEST, RESPONSE> afterRule)
		where REQUEST : IRequest<RESPONSE>
		=> @this.AddSingleton<IAfterRule<REQUEST, RESPONSE>>(afterRule);

	/// <summary>
	/// Registers: <c><see cref="IAfterRule{REQUEST}"/></c>.<br/>
	/// Requires registering rule: <c><see cref="IRule{REQUEST}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddAfterRule<REQUEST>(this IServiceCollection @this, Func<IServiceProvider, IAfterRule<REQUEST>> createAfterRule, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
		where REQUEST : IRequest
		=> @this.AddServiceDescriptor<IAfterRule<REQUEST>>(serviceLifetime, createAfterRule);

	/// <summary>
	/// Registers: <c><see cref="IAfterRule{REQUEST}"/></c>.<br/>
	/// Requires registering rule: <c><see cref="IRule{REQUEST}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddAfterRule<REQUEST>(this IServiceCollection @this, Func<REQUEST, CancellationToken, Task> afterRule, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
		where REQUEST : IRequest
		=> @this.AddServiceDescriptor<IAfterRule<REQUEST>>(serviceLifetime, provider => RuleFactory.CreateAfterRule(afterRule));

	/// <summary>
	/// Registers: <c><see cref="IAfterRule{REQUEST}"/></c>.<br/>
	/// Requires registering rule: <c><see cref="IRule{REQUEST}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddAfterRule<REQUEST>(this IServiceCollection @this, Action<REQUEST> afterRule, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
		where REQUEST : IRequest
		=> @this.AddServiceDescriptor<IAfterRule<REQUEST>>(serviceLifetime, provider => RuleFactory.CreateAfterRule(afterRule));

	/// <summary>
	/// Registers: <c><see cref="IAfterRule{REQUEST, RESPONSE}"/></c>.<br/>
	/// Requires registering rule: <c><see cref="IRule{REQUEST}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddAfterRule<REQUEST, RESPONSE>(this IServiceCollection @this, Func<IServiceProvider, IAfterRule<REQUEST, RESPONSE>> createAfterRule, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
		where REQUEST : IRequest<RESPONSE>
		=> @this.AddServiceDescriptor<IAfterRule<REQUEST, RESPONSE>>(serviceLifetime, createAfterRule);

	/// <summary>
	/// Registers: <c><see cref="IAfterRule{REQUEST, RESPONSE}"/></c>.<br/>
	/// Requires registering rule: <c><see cref="IRule{REQUEST}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddAfterRule<REQUEST, RESPONSE>(this IServiceCollection @this, Func<REQUEST, RESPONSE, CancellationToken, Task> afterRule, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
		where REQUEST : IRequest<RESPONSE>
		=> @this.AddServiceDescriptor<IAfterRule<REQUEST, RESPONSE>>(serviceLifetime, provider => RuleFactory.CreateAfterRule(afterRule));

	/// <summary>
	/// Registers: <c><see cref="IAfterRule{REQUEST, RESPONSE}"/></c>.<br/>
	/// Requires registering rule: <c><see cref="IRule{REQUEST}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddAfterRule<REQUEST, RESPONSE>(this IServiceCollection @this, Action<REQUEST, RESPONSE> afterRule, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
		where REQUEST : IRequest<RESPONSE>
		=> @this.AddServiceDescriptor<IAfterRule<REQUEST, RESPONSE>>(serviceLifetime, provider => RuleFactory.CreateAfterRule(afterRule));

	/// <summary>
	/// Registers keyed singleton: <c><see cref="IDataSource"/></c>.<br/>
	/// </summary>
	public static IServiceCollection AddDataSource(this IServiceCollection @this, string name, DbProviderFactory dbProviderFactory, string connectionString, string[] databases)
		=> @this.AddSingleton<IDataSource>(new DataSource(name, dbProviderFactory, connectionString, databases));

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
	/// Registers Singleton: <c><see cref="IMediator"/></c><br/>
	/// <c><paramref name="rulesBuilder"/></c> - manually register Rules, Validation Rules and After Rules.
	/// </summary>
	public static IServiceCollection AddMediation(this IServiceCollection @this)
		=> @this.AddSingleton<IMediator, Mediator>();

	/// <summary>
	/// Registers: <c><see cref="IRule{REQUEST}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddRule<REQUEST, RULE>(this IServiceCollection @this, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
		where REQUEST : IRequest
		where RULE : class, IRule<REQUEST>
		=> @this.AddServiceDescriptor<IRule<REQUEST>, RULE>(serviceLifetime);

	/// <summary>
	/// Registers singleton: <c><see cref="IRule{REQUEST}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddRule<REQUEST>(this IServiceCollection @this, IRule<REQUEST> rule)
		where REQUEST : IRequest
		=> @this.AddSingleton<IRule<REQUEST>>(rule);

	/// <summary>
	/// Registers: <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddRule<REQUEST, RESPONSE, RULE>(this IServiceCollection @this, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
		where REQUEST : IRequest<RESPONSE>
		where RULE : class, IRule<REQUEST, RESPONSE>
		=> @this.AddServiceDescriptor<IRule<REQUEST, RESPONSE>, RULE>(serviceLifetime);

	/// <summary>
	/// Registers singleton: <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddRule<REQUEST, RESPONSE>(this IServiceCollection @this, IRule<REQUEST, RESPONSE> rule)
		where REQUEST : IRequest<RESPONSE>
		=> @this.AddSingleton<IRule<REQUEST, RESPONSE>>(rule);

	/// <summary>
	/// Registers: <c><see cref="IRule{REQUEST}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddRule<REQUEST>(this IServiceCollection @this, Func<IServiceProvider, IRule<REQUEST>> createRule, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
		where REQUEST : IRequest
		=> @this.AddServiceDescriptor<IRule<REQUEST>>(serviceLifetime, createRule);

	/// <summary>
	/// Registers: <c><see cref="IRule{REQUEST}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddRule<REQUEST>(this IServiceCollection @this, Func<REQUEST, CancellationToken, Task> rule, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
		where REQUEST : IRequest
		=> @this.AddServiceDescriptor<IRule<REQUEST>>(serviceLifetime, provider => RuleFactory.CreateRule(rule));

	/// <summary>
	/// Registers: <c><see cref="IRule{REQUEST}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddRule<REQUEST>(this IServiceCollection @this, Action<REQUEST> rule, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
		where REQUEST : IRequest
		=> @this.AddServiceDescriptor<IRule<REQUEST>>(serviceLifetime, provider => RuleFactory.CreateRule(rule));

	/// <summary>
	/// Registers: <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddRule<REQUEST, RESPONSE>(this IServiceCollection @this, Func<IServiceProvider, IRule<REQUEST, RESPONSE>> createRule, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
		where REQUEST : IRequest<RESPONSE>
		=> @this.AddServiceDescriptor<IRule<REQUEST, RESPONSE>>(serviceLifetime, createRule);

	/// <summary>
	/// Registers: <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddRule<REQUEST, RESPONSE>(this IServiceCollection @this, Func<REQUEST, CancellationToken, Task<RESPONSE>> rule, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
		where REQUEST : IRequest<RESPONSE>
		=> @this.AddServiceDescriptor<IRule<REQUEST, RESPONSE>>(serviceLifetime, provider => RuleFactory.CreateRule(rule));

	/// <summary>
	/// Registers: <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddRule<REQUEST, RESPONSE>(this IServiceCollection @this, Func<REQUEST, RESPONSE> rule, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
		where REQUEST : IRequest<RESPONSE>
		=> @this.AddServiceDescriptor<IRule<REQUEST, RESPONSE>>(serviceLifetime, provider => RuleFactory.CreateRule(rule));

	public static IServiceCollection AddServiceDescriptor<SERVICE, IMPLEMENTATION>(this IServiceCollection @this, ServiceLifetime serviceLifetime)
	{
		@this.Add(ServiceDescriptor.Describe(typeof(SERVICE), typeof(IMPLEMENTATION), serviceLifetime));
		return @this;
	}

	public static IServiceCollection AddServiceDescriptor<T>(this IServiceCollection @this, ServiceLifetime serviceLifetime, Func<IServiceProvider, T> factory)
		where T : class
	{
		@this.Add(ServiceDescriptor.Describe(typeof(T), factory, serviceLifetime));
		return @this;
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.AddSingleton&lt;IRule&lt;<see cref="SqlDataSetRequest"/>, <see cref="DataSet"/>&gt;, <see cref="SqlDataSetRule"/>&gt;()<br/>
	/// <see langword="    "/>.AddSingleton&lt;IRule&lt;<see cref="SqlDataTableRequest"/>, <see cref="DataTable"/>&gt;, <see cref="SqlDataTableRule"/>&gt;()<br/>
	/// <see langword="    "/>.AddSingleton&lt;IRule&lt;<see cref="SqlExecuteRequest"/>, <see cref="SqlExecuteRule"/>&gt;()<br/>
	/// <see langword="    "/>.AddSingleton&lt;IRule&lt;<see cref="SqlJsonArrayRequest"/>, <see cref="JsonArray"/>&gt;, <see cref="SqlJsonArrayRule"/>&gt;()<br/>
	/// <see langword="    "/>.AddSingleton&lt;IRule&lt;<see cref="SqlModelsRequest"/>, IList&lt;<see cref="object"/>&gt;&gt;, <see cref="SqlModelsRule"/>&gt;()<br/>
	/// <see langword="    "/>.AddSingleton&lt;IRule&lt;<see cref="SqlScalarRequest"/>, <see cref="object"/>&gt;, <see cref="SqlScalarRule"/>&gt;()<br/>
	/// </c>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/><br/>
	/// </code>
	/// </summary>
	public static IServiceCollection AddSqlCommandRules(this IServiceCollection @this)
		=> @this.AddRule<SqlDataSetRequest, DataSet>(new SqlDataSetRule())
			.AddRule<SqlDataSetRequest, DataSet>(new SqlDataSetRule())
			.AddRule<SqlDataTableRequest, DataTable>(new SqlDataTableRule())
			.AddRule<SqlExecuteRequest>(new SqlExecuteRule())
			.AddRule<SqlJsonArrayRequest, JsonArray>(new SqlJsonArrayRule())
			.AddRule<SqlModelsRequest, IList<object>>(new SqlModelsRule())
			.AddRule<SqlScalarRequest, object?>(new SqlScalarRule());

	/// <summary>
	/// Registers all types in the specified assembly that have <see cref="ServiceLifetimeAttribute{T}"/> or <see cref="ServiceLifetimeAttribute"/>.
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
				return ServiceDescriptor.Describe(attribute.ServiceType ?? implementationType, implementationType, attribute.ServiceLifetime);
			})
			.ToArray();
		serviceDescriptors.ForEach(@this.Add);

		return @this;
	}

	/// <summary>
	/// Registers singleton: <c><see cref="IValidationRule{REQUEST}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddValidationRule<REQUEST>(this IServiceCollection @this, IValidationRule<REQUEST> validationRule)
		where REQUEST : IRequest
		=> @this.AddSingleton<IValidationRule<REQUEST>>(validationRule);

	/// <summary>
	/// Registers singleton: <c><see cref="IValidationRule{REQUEST}"/></c>.<br/>
	/// Requires registering rule: <c><see cref="IRule{REQUEST}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddValidationRule<REQUEST>(this IServiceCollection @this, Func<REQUEST, CancellationToken, Task> validationRule)
		where REQUEST : IRequest
		=> @this.AddServiceDescriptor<IValidationRule<REQUEST>>(ServiceLifetime.Singleton, provider => RuleFactory.CreateValidationRule(validationRule));

	/// <summary>
	/// Registers singleton: <c><see cref="IValidationRule{REQUEST}"/></c>.<br/>
	/// Requires registering rule: <c><see cref="IRule{REQUEST}"/></c>.<br/>
	/// Requires call to: <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection)"/>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddValidationRule<REQUEST>(this IServiceCollection @this, Action<REQUEST> validationRule)
		where REQUEST : IRequest
		=> @this.AddServiceDescriptor<IValidationRule<REQUEST>>(ServiceLifetime.Singleton, provider => RuleFactory.CreateValidationRule(validationRule));
}
