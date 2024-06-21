// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;

namespace TypeCache.Mediation;

public static class RuleFactory
{
	/// <exception cref="ArgumentNullException"/>
	public static IAfterRule<REQUEST> CreateAfterRule<REQUEST>(Func<REQUEST, CancellationToken, Task> handleAsync)
		where REQUEST : IRequest
	{
		handleAsync.ThrowIfNull();

		return new CustomAfterRule<REQUEST>(handleAsync);
	}

	/// <exception cref="ArgumentNullException"/>
	public static IAfterRule<REQUEST> CreateAfterRule<REQUEST>(Action<REQUEST> handle)
		where REQUEST : IRequest
	{
		handle.ThrowIfNull();

		return new CustomAfterRule<REQUEST>((REQUEST request, CancellationToken token) => Task.Run(() => handle(request), token));
	}

	/// <exception cref="ArgumentNullException"/>
	public static IAfterRule<REQUEST, RESPONSE> CreateAfterRule<REQUEST, RESPONSE>(Func<REQUEST, RESPONSE, CancellationToken, Task> handleAsync)
		where REQUEST : IRequest<RESPONSE>
	{
		handleAsync.ThrowIfNull();

		return new CustomAfterRule<REQUEST, RESPONSE>(handleAsync);
	}

	/// <exception cref="ArgumentNullException"/>
	public static IAfterRule<REQUEST, RESPONSE> CreateAfterRule<REQUEST, RESPONSE>(Action<REQUEST, RESPONSE> handle)
		where REQUEST : IRequest<RESPONSE>
	{
		handle.ThrowIfNull();

		return new CustomAfterRule<REQUEST, RESPONSE>((REQUEST request, RESPONSE response, CancellationToken token) => Task.Run(() => handle(request, response), token));
	}

	/// <exception cref="ArgumentNullException"/>
	public static IRule<REQUEST> CreateRule<REQUEST>(Func<REQUEST, CancellationToken, Task> executeAsync)
		where REQUEST : IRequest
	{
		executeAsync.ThrowIfNull();

		return new CustomRule<REQUEST>(executeAsync);
	}

	/// <exception cref="ArgumentNullException"/>
	public static IRule<REQUEST> CreateRule<REQUEST>(Action<REQUEST> execute)
		where REQUEST : IRequest
	{
		execute.ThrowIfNull();

		return new CustomRule<REQUEST>((REQUEST request, CancellationToken token) => Task.Run(() => execute(request), token));
	}

	/// <exception cref="ArgumentNullException"/>
	public static IRule<REQUEST, RESPONSE> CreateRule<REQUEST, RESPONSE>(Func<REQUEST, CancellationToken, Task<RESPONSE>> mapAsync)
		where REQUEST : IRequest<RESPONSE>
	{
		mapAsync.ThrowIfNull();

		return new CustomRule<REQUEST, RESPONSE>(mapAsync);
	}

	/// <exception cref="ArgumentNullException"/>
	public static IRule<REQUEST, RESPONSE> CreateRule<REQUEST, RESPONSE>(Func<REQUEST, RESPONSE> map)
		where REQUEST : IRequest<RESPONSE>
	{
		map.ThrowIfNull();

		return new CustomRule<REQUEST, RESPONSE>((REQUEST request, CancellationToken token) => Task.Run(() => map(request), token));
	}

	/// <exception cref="ArgumentNullException"/>
	public static IValidationRule<REQUEST> CreateValidationRule<REQUEST>(Func<REQUEST, CancellationToken, Task> validateAsync)
		where REQUEST : IRequest
	{
		validateAsync.ThrowIfNull();

		return new CustomValidationRule<REQUEST>(validateAsync);
	}

	/// <exception cref="ArgumentNullException"/>
	public static IValidationRule<REQUEST> CreateValidationRule<REQUEST>(Action<REQUEST> validate)
		where REQUEST : IRequest
	{
		validate.ThrowIfNull();

		return new CustomValidationRule<REQUEST>((request, token) => Task.Run(() => validate(request), token));
	}
}
