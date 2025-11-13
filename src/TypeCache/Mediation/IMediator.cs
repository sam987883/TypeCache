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
	Task Dispatch<REQUEST>(REQUEST request, CancellationToken token = default)
		where REQUEST : notnull;

	/// <summary>
	/// Execute a keyed rule with no response.<br/>
	/// Must register a keyed rule of type <c><see cref="IRule{REQUEST}"/></c>.<br/>
	/// May register keyed and/or non-keyed rules of type <c><see cref="IValidationRule{REQUEST}"/></c> that run in parallel before the <c><see cref="IRule{REQUEST}"/></c> rule runs.<br/>
	/// May register keyed and/or non-keyed rules of type <c><see cref="IAfterRule{REQUEST}"/></c> that run in parallel after the <c><see cref="IRule{REQUEST}"/></c> rule runs.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	Task Dispatch<REQUEST>(object key, REQUEST request, CancellationToken token = default)
		where REQUEST : notnull;

	/// <summary>
	/// Prepares an executor of a keyed/non-keyed rule with a response of type <c><typeparamref name="RESPONSE"/></c>.<br/>
	/// Must register a rule of type <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.<br/>
	/// May register rules of type <c><see cref="IValidationRule{REQUEST}"/></c> that run in parallel before the <c><see cref="IRule{REQUEST}"/></c> rule runs.<br/>
	/// May register rules of type <c><see cref="IAfterRule{REQUEST}"/></c> that run in parallel after the <c><see cref="IRule{REQUEST, RESPONSE}"/></c> rule runs.
	/// </summary>
	ISend<RESPONSE> Request<RESPONSE>(object? key = null);

	/// <summary>
	/// Execute a rule with a response of type <c><typeparamref name="RESPONSE"/></c>.<br/>
	/// Must register a rule of type <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.<br/>
	/// May register rules of type <c><see cref="IValidationRule{REQUEST}"/></c> that run in parallel before the <c><see cref="IRule{REQUEST}"/></c> rule runs.<br/>
	/// May register rules of type <c><see cref="IAfterRule{REQUEST}"/></c> that run in parallel after the <c><see cref="IRule{REQUEST, RESPONSE}"/></c> rule runs.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	ValueTask<RESPONSE> Send<RESPONSE>(IRequest<RESPONSE> request, CancellationToken token = default);

	/// <summary>
	/// Execute a keyed rule with a response of type <c><typeparamref name="RESPONSE"/></c>.<br/>
	/// Must register a keyed rule of type <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.<br/>
	/// May register keyed and/or non-keyed rules of type <c><see cref="IValidationRule{REQUEST}"/></c> that run in parallel before the <c><see cref="IRule{REQUEST}"/></c> rule runs.<br/>
	/// May register keyed and/or non-keyed rules of type <c><see cref="IAfterRule{REQUEST}"/></c> that run in parallel after the <c><see cref="IRule{REQUEST, RESPONSE}"/></c> rule runs.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	ValueTask<RESPONSE> Send<RESPONSE>(object key, IRequest<RESPONSE> request, CancellationToken token = default);

	/// <summary>
	/// Validates a rule request.  If no exception is thrown, the request is deemed valid.<br/>
	/// Must register validation rules of type <c><see cref="IValidationRule{REQUEST}"/></c>.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	void Validate<REQUEST>(REQUEST request)
		where REQUEST : notnull;

	/// <summary>
	/// Validates a keyed rule request.  If no exception is thrown, the request is deemed valid.<br/>
	/// Must register keyed or non-keyed validation rules of type <c><see cref="IValidationRule{REQUEST}"/></c>.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	void Validate<REQUEST>(object key, REQUEST request)
		where REQUEST : notnull;
}
