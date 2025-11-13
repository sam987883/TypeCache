// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;

namespace TypeCache.Mediation;

public static class RuleFactory
{
	/// <exception cref="ArgumentNullException"/>
	public static IAfterRule<REQUEST> CreateAfterRule<REQUEST>(Func<REQUEST, CancellationToken, Task> handleAsync)
		where REQUEST : notnull
	{
		handleAsync.ThrowIfNull();

		return new CustomAfterRule<REQUEST>(handleAsync);
	}

	/// <exception cref="ArgumentNullException"/>
	public static IAfterRule<REQUEST> CreateAfterRule<REQUEST>(Action<REQUEST> handle)
		where REQUEST : notnull
	{
		handle.ThrowIfNull();

		return new CustomAfterRule<REQUEST>((REQUEST request, CancellationToken token) => Task.Run(() => handle(request), token));
	}

	/// <exception cref="ArgumentNullException"/>
	public static IRule<REQUEST> CreateRule<REQUEST>(Func<REQUEST, CancellationToken, Task> executeAsync)
		where REQUEST : notnull
	{
		executeAsync.ThrowIfNull();

		return new CustomRule<REQUEST>(executeAsync);
	}

	/// <exception cref="ArgumentNullException"/>
	public static IRule<REQUEST> CreateRule<REQUEST>(Action<REQUEST> execute)
		where REQUEST : notnull
	{
		execute.ThrowIfNull();

		return new CustomRule<REQUEST>((REQUEST request, CancellationToken token)
			=> Task.Run(() => execute(request), token));
	}

	/// <exception cref="ArgumentNullException"/>
	public static IRule<REQUEST> CreateRule<REQUEST>(Action<REQUEST, CancellationToken> execute)
		where REQUEST : notnull
	{
		execute.ThrowIfNull();

		return new CustomRule<REQUEST>((REQUEST request, CancellationToken token)
			=> Task.Run(() => execute(request, token), token));
	}

	/// <exception cref="ArgumentNullException"/>
	public static IRule<REQUEST, RESPONSE> CreateRule<REQUEST, RESPONSE>(Func<REQUEST, CancellationToken, RESPONSE> mapAsync)
		where REQUEST : notnull
	{
		mapAsync.ThrowIfNull();

		return new CustomRule<REQUEST, RESPONSE>((REQUEST request, CancellationToken token)
			=> new ValueTask<RESPONSE>(Task.Run(() => mapAsync(request, token), token)));
	}

	/// <exception cref="ArgumentNullException"/>
	public static IRule<REQUEST, RESPONSE> CreateRule<REQUEST, RESPONSE>(Func<REQUEST, RESPONSE> map)
		where REQUEST : notnull
	{
		map.ThrowIfNull();

		return new CustomRule<REQUEST, RESPONSE>((REQUEST request, CancellationToken token)
			=> new ValueTask<RESPONSE>(Task.Run(() => map(request))));
	}

	/// <exception cref="ArgumentNullException"/>
	public static IValidationRule<REQUEST> CreateValidationRule<REQUEST>(Action<REQUEST> validate)
		where REQUEST : notnull
	{
		validate.ThrowIfNull();

		return new CustomValidationRule<REQUEST>(validate);
	}
}
