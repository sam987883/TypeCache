// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Attributes;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Mediation;

public static class Extensions
{
	extension(IServiceCollection @this)
	{
		/// <summary>
		/// Registers all mediation rules in the specified assembly, honoring any of the following service lifetime scope attributes:
		/// <list type="bullet">
		/// <item><c><see cref="SingletonAttribute"/></c></item>
		/// <item><c><see cref="ScopedAttribute"/></c></item>
		/// <item><c><see cref="TransientAttribute"/></c></item>
		/// </list>
		/// </summary>
		/// <param name="fromAssembly">The assembly to register the types from.</param>
		public IServiceCollection AddMediationRules(Assembly fromAssembly)
		{
			fromAssembly.GetTypes()
				.Where(type => !type.IsAbstract && !type.IsGenericType && !type.IsInterface && !type.IsPointer && !type.IsPrimitive
					&& type.Implements(typeof(IRule<,>)))
				.ForEach(ruleType => @this.AddRule(ruleType));

			return @this;
		}

		/// <summary>
		/// Registers singleton: <c><see cref="IMediator"/></c><br/>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public IServiceCollection AddMediator()
			=> @this.AddSingleton<IMediator, Mediator>();

		/// <summary>
		/// Registers: <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.<br/>
		/// Requires call to: <see cref="Extensions.AddMediator(IServiceCollection)"/>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public IServiceCollection AddRule<RULE>(object? key = null)
			where RULE : class
			=> @this.AddRule(typeof(RULE));

		/// <summary>
		/// Registers: <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.<br/>
		/// Requires call to: <see cref="Extensions.AddMediator(IServiceCollection)"/>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		public IServiceCollection AddRule(Type ruleType, object? key = null)
		{
			if (!ruleType.GetInterfaces().Where(_ => _.Implements(typeof(IRule<,>))).TrySingle(out var serviceType))
				throw new InvalidOperationException(Invariant($"Type {ruleType.CodeName} does not implement {typeof(IRule<,>).CodeName}."));

			var method = (typeof(Extensions) | nameof(CreateRule))
				.Find([.. serviceType.GenericTypeArguments, ruleType], [typeof(IServiceProvider), typeof(object)])!;
			var serviceLifetime = ruleType.DeclaredAttributes.ServiceLifetime ?? ServiceLifetime.Singleton;

			@this.Add(ServiceDescriptor.DescribeKeyed(ruleType, key, ruleType, serviceLifetime));
			@this.Add(ServiceDescriptor.DescribeKeyed(serviceType, key,
				(provider, key) => provider.Scope(provider => method & (provider, key))!, serviceLifetime));

			return @this;
		}

		/// <summary>
		/// Registers a named implementation of <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.<br/>
		/// Requires call to: <see cref="Extensions.AddMediator(IServiceCollection)"/>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		public IServiceCollection AddRule<REQUEST, RESPONSE>(Func<REQUEST, CancellationToken, RESPONSE> rule, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
			where REQUEST : notnull
		{
			rule.ThrowIfNull();
			return @this.AddServiceDescriptor<IRule<REQUEST, RESPONSE>>(serviceLifetime,
				provider => provider.Scope(provider => provider.CreateRule(null, rule)));
		}

		/// <summary>
		/// Registers a named implementation of <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.<br/>
		/// Requires call to: <see cref="Extensions.AddMediator(IServiceCollection)"/>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		public IServiceCollection AddRule<REQUEST, RESPONSE>(Func<IServiceProvider, REQUEST, CancellationToken, RESPONSE> rule, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
			where REQUEST : notnull
		{
			rule.ThrowIfNull();
			return @this.AddServiceDescriptor<IRule<REQUEST, RESPONSE>>(serviceLifetime,
				provider => provider.Scope(provider => provider.CreateRule<REQUEST, RESPONSE>(null, (request, token) => rule(provider, request, token))));
		}

		/// <summary>
		/// Registers a named implementation of <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.<br/>
		/// Requires call to: <see cref="Extensions.AddMediator(IServiceCollection)"/>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		public IServiceCollection AddRule<REQUEST, RESPONSE>(Func<object, REQUEST, CancellationToken, RESPONSE> rule, object key, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
			where REQUEST : notnull
		{
			rule.ThrowIfNull();
			key.ThrowIfNull();
			return @this.AddServiceDescriptor<IRule<REQUEST, RESPONSE>>(key, serviceLifetime,
				(provider, key) => provider.Scope(provider => provider.CreateRule<REQUEST, RESPONSE>(key, (request, token) => rule(key!, request, token))));
		}

		/// <summary>
		/// Registers a named implementation of <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.<br/>
		/// Requires call to: <see cref="Extensions.AddMediator(IServiceCollection)"/>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		public IServiceCollection AddRule<REQUEST, RESPONSE>(Func<IServiceProvider, object, REQUEST, CancellationToken, RESPONSE> rule, object key, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
			where REQUEST : notnull
		{
			rule.ThrowIfNull();
			key.ThrowIfNull();
			return @this.AddServiceDescriptor<IRule<REQUEST, RESPONSE>>(key, serviceLifetime,
				(provider, key) => provider.Scope(provider => new CustomRule<REQUEST, RESPONSE>((request, token) => rule(provider, key!, request, token))));
		}

		/// <summary>
		/// Registers: <c><see cref="IRuleHandler{REQUEST, RESPONSE}"/></c>.<br/>
		/// Requires call to: <see cref="Extensions.AddMediator(IServiceCollection)"/>
		/// </summary>
		/// <exception cref="ArgumentNullException"/>
		public IServiceCollection AddRuleHandler<REQUEST, RESPONSE, HANDLER>(object? key = null)
			where REQUEST : notnull
			where HANDLER : class, IRuleHandler<REQUEST, RESPONSE>
		{
			var serviceLifetime = Type<HANDLER>.Attributes.ServiceLifetime ?? ServiceLifetime.Singleton;
			return @this.AddServiceDescriptor<IRuleHandler<REQUEST, RESPONSE>, HANDLER>(serviceLifetime, key);
		}
	}

	extension(IServiceProvider @this)
	{
		internal IRule<REQUEST, RESPONSE> CreateRule<REQUEST, RESPONSE>(object? key,
			Func<REQUEST, CancellationToken, RESPONSE> rule)
			where REQUEST : notnull
		{
			rule.ThrowIfNull();

			var ruleHandlers = @this.GetKeyedServices<IRuleHandler<REQUEST, RESPONSE>>(key);
			if (!ruleHandlers.Any())
				ruleHandlers.Reverse().ForEach(handler => rule = (request, token) => handler.Handle(request, token, rule));

			return new CustomRule<REQUEST, RESPONSE>(rule);
		}

		internal IRule<REQUEST, RESPONSE> CreateRule<REQUEST, RESPONSE, RULE>(object? key)
			where REQUEST : notnull
			where RULE : class, IRule<REQUEST, RESPONSE>
		{
			var rule = @this.GetRequiredKeyedService<RULE>(key);
			return @this.CreateRule<REQUEST, RESPONSE>(key, rule.Send);
		}
	}

	extension<REQUEST>(REQUEST @this)
		where REQUEST : notnull
	{
		/// <summary>
		/// Creates a request object for use with <c><see cref="IMediator"/></c>.
		/// </summary>
		/// <returns>A request instance to pass to <c><see cref="IMediator.Send{REQUEST, RESPONSE}(Request{REQUEST, RESPONSE}, CancellationToken)"/></c>
		/// or <c><see cref="IMediator.Dispatch{REQUEST}(Request{REQUEST}, System.Threading.CancellationToken)"/></c></returns>
		public Request<REQUEST> Request
			=> new(@this, null);
	}
}
