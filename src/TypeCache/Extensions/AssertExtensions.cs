// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;

namespace TypeCache.Extensions;

public static class AssertExtensions
{
	private const string NULL = "null";

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertEquals<T>(this T @this, T value,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
		where T : struct
	{
		if (!EqualityComparer<T>.Default.Equals(@this, value))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this}.{nameof(AssertEquals)}<{TypeOf<T>.Name}>({value})."));
	}

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertEquals<T>(this T? @this, T? value, IEqualityComparer<T> comparer,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
		where T : notnull
	{
		comparer.AssertNotNull(caller: caller);

		if (!comparer.Equals(@this, value))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this?.ToString() ?? NULL}.{nameof(AssertEquals)}<{TypeOf<T>.Name}>({value?.ToString() ?? NULL})."));
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.AssertEquals(<paramref name="value"/>, <paramref name="comparison"/>.ToStringComparer(), <paramref name="argument"/>, <paramref name="caller"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertEquals(this string? @this, string? value, StringComparison comparison = StringComparison.OrdinalIgnoreCase,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
		=> @this.AssertEquals(value, comparison.ToStringComparer(), argument, caller);

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertFalse(this bool @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (@this)
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {nameof(AssertFalse)}({argument})."));
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertNotSame(this object? @this, object? value, StringComparison comparison = StringComparison.OrdinalIgnoreCase,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (object.ReferenceEquals(@this, value))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this?.ToString() ?? NULL}.{nameof(AssertNotSame)}({value?.ToString() ?? NULL})."));
	}

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertNotBlank([NotNull] this string? @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		@this.AssertNotNull(argument, caller);

		if (@this.IsBlank())
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {argument}.{nameof(AssertNotBlank)}()."));
	}

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertNotEmpty<T>([NotNull] this IEnumerable<T>? @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
		where T : notnull
	{
		@this.AssertNotNull(argument, caller);

		if (!@this.Any())
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {argument}.{nameof(AssertNotEmpty)}<IEnumerable<{TypeOf<T>.Name}>>()."));
	}

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertNotEmpty<T>([NotNull] this T[]? @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
		where T : notnull
	{
		@this.AssertNotNull(argument, caller);

		if (!@this.Any())
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {argument}.{nameof(AssertNotEmpty)}<{TypeOf<T>.Name}[]>()."));
	}

	/// <exception cref="ArgumentNullException"/>
	public static void AssertNotNull<T>([NotNull] this T? @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
		where T : notnull
	{
		if (@this is null)
			throw new ArgumentNullException(argument, Invariant($"{caller}: {argument}.{nameof(AssertNotNull)}<{TypeOf<T>.Name}>()."));
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertNotSame<T>(this (T?, T?) @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
		where T : notnull
	{
		if (object.ReferenceEquals(@this.Item1, @this.Item2))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this.Item1?.ToString() ?? NULL}.{nameof(AssertNotSame)}<{TypeOf<T>.Name}>({@this.Item2?.ToString() ?? NULL})."));
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertSame(this object? @this, object? value,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (object.ReferenceEquals(@this, value))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this?.ToString() ?? NULL}.{nameof(AssertNotSame)}({value?.ToString() ?? NULL})."));
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertTrue(this bool @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (!@this)
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {nameof(AssertTrue)}({argument})."));
	}
}
