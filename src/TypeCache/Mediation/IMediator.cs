// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public interface IMediator
{
	/// <summary>
	/// Execute a rule with no response.<br/>
	/// Must register a rule of type <c><see cref="IRule{REQUEST}"/></c>.<br/>
	/// May register rules of type <c><see cref="IValidationRule{REQUEST}"/></c> that run in parallel before the <c><see cref="IRule{REQUEST}"/></c> rule runs.<br/>
	/// May register rules of type <c><see cref="IAfterRule{REQUEST}"/></c> that run in parallel after the <c><see cref="IRule{REQUEST}"/></c> rule runs.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	Task Execute<REQUEST>(REQUEST request, CancellationToken token = default)
		where REQUEST : IRequest;

	/// <summary>
	/// Execute a named rule with no response.<br/>
	/// Must register a named rule of type <c><see cref="IRule{REQUEST}"/></c>.<br/>
	/// May register named and/or unamed rules of type <c><see cref="IValidationRule{REQUEST}"/></c> that run in parallel before the <c><see cref="IRule{REQUEST}"/></c> rule runs.<br/>
	/// May register named and/or unamed rules of type <c><see cref="IAfterRule{REQUEST}"/></c> that run in parallel after the <c><see cref="IRule{REQUEST}"/></c> rule runs.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	Task Execute<REQUEST>(object? key, REQUEST request, CancellationToken token = default)
		where REQUEST : IRequest;

	/// <summary>
	/// Execute a rule with a response of type <c><typeparamref name="RESPONSE"/></c>.<br/>
	/// Must register a rule of type <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.<br/>
	/// May register rules of type <c><see cref="IValidationRule{REQUEST}"/></c> that run in parallel before the <c><see cref="IRule{REQUEST}"/></c> rule runs.<br/>
	/// May register rules of type <c><see cref="IAfterRule{REQUEST, RESPONSE}"/></c> that run in parallel after the <c><see cref="IRule{REQUEST, RESPONSE}"/></c> rule runs.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	Task<RESPONSE> Map<RESPONSE>(IRequest<RESPONSE> request, CancellationToken token = default);

	/// <summary>
	/// Execute a named rule with a response of type <c><typeparamref name="RESPONSE"/></c>.<br/>
	/// Must register a named rule of type <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.<br/>
	/// May register named and/or unamed rules of type <c><see cref="IValidationRule{REQUEST}"/></c> that run in parallel before the <c><see cref="IRule{REQUEST}"/></c> rule runs.<br/>
	/// May register named and/or unamed rules of type <c><see cref="IAfterRule{REQUEST, RESPONSE}"/></c> that run in parallel after the <c><see cref="IRule{REQUEST, RESPONSE}"/></c> rule runs.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	Task<RESPONSE> Map<RESPONSE>(object? key, IRequest<RESPONSE> request, CancellationToken token = default);

	/// <summary>
	/// Validates a rule request.  If no exception is thrown, the request is deemed valid.<br/>
	/// Must register validation rules of type <c><see cref="IValidationRule{REQUEST}"/></c>.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	void Validate<REQUEST>(REQUEST request, CancellationToken token = default)
		where REQUEST : notnull;
}
