// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;

namespace TypeCache.Mediation;

public readonly struct RulesBuilder
{
	private readonly IServiceCollection _Services;

	public RulesBuilder(IServiceCollection services)
	{
		services.AssertNotNull();

		this._Services = services;
	}

	/// <summary>
	/// Registers Singleton: <c><see cref="IAfterRule{REQUEST}"/></c>.
	/// </summary>
	public RulesBuilder AddAfterRule<REQUEST>(IAfterRule<REQUEST> afterRule)
		where REQUEST : IRequest
	{
		afterRule.AssertNotNull();
		this._Services.AddSingleton<IAfterRule<REQUEST>>(afterRule);
		return this;
	}

	/// <summary>
	/// Registers Singleton: <c><see cref="IAfterRule{REQUEST, RESPONSE}"/></c>.
	/// </summary>
	public RulesBuilder AddAfterRule<REQUEST, RESPONSE>(IAfterRule<REQUEST, RESPONSE> afterRule)
		where REQUEST : IRequest<RESPONSE>
	{
		afterRule.AssertNotNull();
		this._Services.AddSingleton<IAfterRule<REQUEST, RESPONSE>>(afterRule);
		return this;
	}

	/// <summary>
	/// Registers an implementation of <c><see cref="IAfterRule{REQUEST}"/></c>.
	/// </summary>
	public RulesBuilder AddAfterRule<REQUEST>(ServiceLifetime serviceLifetime, Func<IServiceProvider, IAfterRule<REQUEST>> createAfterRule)
		where REQUEST : IRequest
	{
		createAfterRule.AssertNotNull();
		this._Services.Add(ServiceDescriptor.Describe(typeof(IAfterRule<REQUEST>), createAfterRule, serviceLifetime));
		return this;
	}

	/// <summary>
	/// Registers an implementation of <c><see cref="IAfterRule{REQUEST}"/></c>.
	/// </summary>
	public RulesBuilder AddAfterRule<REQUEST>(ServiceLifetime serviceLifetime, Func<REQUEST, CancellationToken, Task> afterRule)
		where REQUEST : IRequest
	{
		afterRule.AssertNotNull();
		this._Services.Add(ServiceDescriptor.Describe(typeof(IAfterRule<REQUEST>), provider => RuleFactory.CreateAfterRule(afterRule), serviceLifetime));
		return this;
	}

	/// <summary>
	/// Registers an implementation of <c><see cref="IAfterRule{REQUEST}"/></c>.
	/// </summary>
	public RulesBuilder AddAfterRule<REQUEST>(ServiceLifetime serviceLifetime, Action<REQUEST> afterRule)
		where REQUEST : IRequest
	{
		afterRule.AssertNotNull();
		this._Services.Add(ServiceDescriptor.Describe(typeof(IAfterRule<REQUEST>), provider => RuleFactory.CreateAfterRule(afterRule), serviceLifetime));
		return this;
	}

	/// <summary>
	/// Registers an implementation of <c><see cref="IAfterRule{REQUEST, RESPONSE}"/></c>.
	/// </summary>
	public RulesBuilder AddAfterRule<REQUEST, RESPONSE>(ServiceLifetime serviceLifetime, Func<IServiceProvider, IAfterRule<REQUEST, RESPONSE>> createAfterRule)
		where REQUEST : IRequest<RESPONSE>
	{
		createAfterRule.AssertNotNull();
		this._Services.Add(ServiceDescriptor.Describe(typeof(IAfterRule<REQUEST, RESPONSE>), createAfterRule, serviceLifetime));
		return this;
	}

	/// <summary>
	/// Registers an implementation of <c><see cref="IAfterRule{REQUEST, RESPONSE}"/></c>.
	/// </summary>
	public RulesBuilder AddAfterRule<REQUEST, RESPONSE>(ServiceLifetime serviceLifetime, Func<REQUEST, RESPONSE, CancellationToken, Task> afterRule)
		where REQUEST : IRequest<RESPONSE>
	{
		afterRule.AssertNotNull();
		this._Services.Add(ServiceDescriptor.Describe(typeof(IAfterRule<REQUEST, RESPONSE>), provider => RuleFactory.CreateAfterRule(afterRule), serviceLifetime));
		return this;
	}

	/// <summary>
	/// Registers an implementation of <c><see cref="IAfterRule{REQUEST, RESPONSE}"/></c>.
	/// </summary>
	public RulesBuilder AddAfterRule<REQUEST, RESPONSE>(ServiceLifetime serviceLifetime, Action<REQUEST, RESPONSE> afterRule)
		where REQUEST : IRequest<RESPONSE>
	{
		afterRule.AssertNotNull();
		this._Services.Add(ServiceDescriptor.Describe(typeof(IAfterRule<REQUEST, RESPONSE>), provider => RuleFactory.CreateAfterRule(afterRule), serviceLifetime));
		return this;
	}

	/// <summary>
	/// Registers Singleton: <c><see cref="IRule{REQUEST}"/></c>.
	/// </summary>
	public RulesBuilder AddRule<REQUEST>(IRule<REQUEST> rule)
		where REQUEST : IRequest
	{
		rule.AssertNotNull();
		this._Services.AddSingleton<IRule<REQUEST>>(rule);
		return this;
	}

	/// <summary>
	/// Registers Singleton: <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.
	/// </summary>
	public RulesBuilder AddRule<REQUEST, RESPONSE>(IRule<REQUEST, RESPONSE> rule)
		where REQUEST : IRequest<RESPONSE>
	{
		rule.AssertNotNull();
		this._Services.AddSingleton<IRule<REQUEST, RESPONSE>>(rule);
		return this;
	}

	/// <summary>
	/// Registers an implementation of <c><see cref="IRule{REQUEST}"/></c>.
	/// </summary>
	public RulesBuilder AddRule<REQUEST>(ServiceLifetime serviceLifetime, Func<IServiceProvider, IRule<REQUEST>> createRule)
		where REQUEST : IRequest
	{
		createRule.AssertNotNull();
		this._Services.Add(ServiceDescriptor.Describe(typeof(IRule<REQUEST>), createRule, serviceLifetime));
		return this;
	}

	/// <summary>
	/// Registers an implementation of <c><see cref="IRule{REQUEST}"/></c>.
	/// </summary>
	public RulesBuilder AddRule<REQUEST>(ServiceLifetime serviceLifetime, Func<REQUEST, CancellationToken, Task> rule)
		where REQUEST : IRequest
	{
		rule.AssertNotNull();
		this._Services.Add(ServiceDescriptor.Describe(typeof(IRule<REQUEST>), provider => RuleFactory.CreateRule(rule), serviceLifetime));
		return this;
	}

	/// <summary>
	/// Registers an implementation of <c><see cref="IRule{REQUEST}"/></c>.
	/// </summary>
	public RulesBuilder AddRule<REQUEST>(ServiceLifetime serviceLifetime, Action<REQUEST> rule)
		where REQUEST : IRequest
	{
		rule.AssertNotNull();
		this._Services.Add(ServiceDescriptor.Describe(typeof(IRule<REQUEST>), provider => RuleFactory.CreateRule(rule), serviceLifetime));
		return this;
	}

	/// <summary>
	/// Registers an implementation of <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.
	/// </summary>
	public RulesBuilder AddRule<REQUEST, RESPONSE>(ServiceLifetime serviceLifetime, Func<IServiceProvider, IRule<REQUEST, RESPONSE>> createRule)
		where REQUEST : IRequest<RESPONSE>
	{
		createRule.AssertNotNull();
		this._Services.Add(ServiceDescriptor.Describe(typeof(IRule<REQUEST, RESPONSE>), createRule, serviceLifetime));
		return this;
	}

	/// <summary>
	/// Registers an implementation of <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.
	/// </summary>
	public RulesBuilder AddRule<REQUEST, RESPONSE>(ServiceLifetime serviceLifetime, Func<REQUEST, CancellationToken, Task<RESPONSE>> rule)
		where REQUEST : IRequest<RESPONSE>
	{
		rule.AssertNotNull();
		this._Services.Add(ServiceDescriptor.Describe(typeof(IRule<REQUEST, RESPONSE>), provider => RuleFactory.CreateRule(rule), serviceLifetime));
		return this;
	}

	/// <summary>
	/// Registers an implementation of <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.
	/// </summary>
	public RulesBuilder AddRule<REQUEST, RESPONSE>(ServiceLifetime serviceLifetime, Func<REQUEST, RESPONSE> rule)
		where REQUEST : IRequest<RESPONSE>
	{
		rule.AssertNotNull();
		this._Services.Add(ServiceDescriptor.Describe(typeof(IRule<REQUEST, RESPONSE>), provider => RuleFactory.CreateRule(rule), serviceLifetime));
		return this;
	}

	/// <summary>
	/// Registers Singleton: <c><see cref="IValidationRule{REQUEST}"/></c>.
	/// </summary>
	public RulesBuilder AddValidationRule<REQUEST>(IValidationRule<REQUEST> validationRule)
		where REQUEST : IRequest
	{
		validationRule.AssertNotNull();
		this._Services.AddSingleton<IValidationRule<REQUEST>>(validationRule);
		return this;
	}

	/// <summary>
	/// Registers an implementation of <c><see cref="IValidationRule{REQUEST}"/></c>.
	/// </summary>
	public RulesBuilder AddValidationRule<REQUEST>(ServiceLifetime serviceLifetime, Func<REQUEST, CancellationToken, Task> validationRule)
		where REQUEST : IRequest
	{
		validationRule.AssertNotNull();
		this._Services.Add(ServiceDescriptor.Describe(typeof(IValidationRule<REQUEST>), provider => RuleFactory.CreateValidationRule(validationRule), serviceLifetime));
		return this;
	}

	/// <summary>
	/// Registers an implementation of <c><see cref="IValidationRule{REQUEST}"/></c>.
	/// </summary>
	public RulesBuilder AddValidationRule<REQUEST>(ServiceLifetime serviceLifetime, Action<REQUEST> validationRule)
		where REQUEST : IRequest
	{
		validationRule.AssertNotNull();
		this._Services.Add(ServiceDescriptor.Describe(typeof(IValidationRule<REQUEST>), provider => RuleFactory.CreateValidationRule(validationRule), serviceLifetime));
		return this;
	}
}
