// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public interface IMediator
{
	/// <summary>
	/// Execute a job.<br/>
	/// Must register a rule of type <c>IRule&lt;REQUEST, <see cref="Task"/>&gt;</c>.<br/>
	/// May register keyed and/or non-keyed rules of type <c><see cref="IValidation{REQUEST}"/></c> that run in parallel before the <c>IRule&lt;REQUEST, <see cref="Task"/>&gt;</c> runs.<br/>
	/// May register handlers of type <c>IRuleHandler&lt;REQUEST, <see cref="Task"/>&gt;</c> that can wrap the call to <c>IRule&lt;REQUEST, <see cref="Task"/>&gt;</c> in middleware fashion.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	Task Dispatch<REQUEST>(Request<REQUEST> request, CancellationToken token = default)
		where REQUEST : notnull
		=> this.Send(request.For<Task>(), token);

	/// <summary>
	/// Execute a rule with a response of type <c><typeparamref name="RESPONSE"/></c>.<br/>
	/// Must register a rule of type <c><see cref="IRule{REQUEST, RESPONSE}"/></c>.<br/>
	/// May register rules of type <c><see cref="IValidation{REQUEST}"/></c> that run in parallel before the <c><see cref="IRule{REQUEST, RESPONSE}"/></c> runs.<br/>
	/// May register handlers of type <c><see cref="IRuleHandler{REQUEST, RESPONSE}"/></c> that can wrap the call to <c><see cref="IRule{REQUEST, RESPONSE}"/></c> in middleware fashion.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	RESPONSE Send<REQUEST, RESPONSE>(Request<REQUEST, RESPONSE> request, CancellationToken token = default)
		where REQUEST : notnull;

	/// <summary>
	/// Validates a job or rule request.  If no exception is thrown, the request is deemed valid.<br/>
	/// Must register keyed/non-keyed validation rules of type <c><see cref="IValidation{REQUEST}"/></c>.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	void Validate<REQUEST>(REQUEST request, object? key = null)
		where REQUEST : notnull;
}
