// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public interface IMediator
{
	/// <summary>
	/// Execute a rule with no response.
	/// Must register a rule of type <see cref="IRule{REQUEST}"/>.
	/// May register rules of type <see cref="IAfterRule{REQUEST}"/>.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	ValueTask ExecuteAsync(IRequest request, CancellationToken token = default);

	/// <summary>
	/// Execute a rule with a response of type <typeparamref name="RESPONSE"/>.
	/// Must register a rule of type <see cref="IRule{REQUEST, RESPONSE}"/>.
	/// May register rules of type <see cref="IAfterRule{REQUEST, RESPONSE}"/>.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	ValueTask<RESPONSE> MapAsync<RESPONSE>(IRequest<RESPONSE> request, CancellationToken token = default);

	/// <summary>
	/// Run a rule on a separate thread, with no response.
	/// Must register a rule of type <see cref="IRule{REQUEST}"/>.
	/// May register rules of type <see cref="IAfterRule{REQUEST}"/>.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	ValueTask PublishAsync(IRequest request, Action onComplete, CancellationToken token = default);

	/// <summary>
	/// Retrieve validation messages for a rule request.
	/// Must register validation rules of type <see cref="IValidationRule{REQUEST}"/>.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	IEnumerable<string> Validate<REQUEST>(REQUEST request);
}
