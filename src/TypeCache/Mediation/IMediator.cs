// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public interface IMediator
{
	/// <summary>
	/// Execute a rule with no response.
	/// Must register a rule of type <see cref="IRule{REQUEST}"/>.
	/// May register rules of type <see cref="IAfterRule{REQUEST}"/> that run in parallel after the <see cref="IRule{REQUEST}"/> rules run.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	Task Execute<REQUEST>(REQUEST request, CancellationToken token = default)
		where REQUEST : IRequest;

	/// <summary>
	/// Execute a rule with a response of type <typeparamref name="RESPONSE"/>.
	/// Must register a rule of type <see cref="IRule{REQUEST, RESPONSE}"/>.
	/// May register rules of type <see cref="IAfterRule{REQUEST, RESPONSE}"/> that run in parallel after the <see cref="IRule{REQUEST, RESPONSE}"/> rules run.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	Task<RESPONSE> Map<RESPONSE>(IRequest<RESPONSE> request, CancellationToken token = default);

	/// <summary>
	/// Retrieve validation messages for a rule request.
	/// Must register validation rules of type <see cref="IValidationRule{REQUEST}"/>.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	void Validate<REQUEST>(REQUEST request, CancellationToken token = default)
		where REQUEST : notnull;
}
