// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;

namespace TypeCache.Extensions;

public static class AssertExtensions
{
	private const string NULL = "null";

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertEquals(this string? @this, string? value, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (!string.Equals(@this, value, comparisonType))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this ?? NULL}.{nameof(AssertEquals)}({value ?? NULL})."));
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertEquals<T>(this T? @this, T? value, IEqualityComparer<T>? comparer = null,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		comparer ??= EqualityComparer<T>.Default;

		if (!comparer.Equals(@this, value))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this?.ToString() ?? NULL}.{nameof(AssertEquals)}<{typeof(T).Name}>({value?.ToString() ?? NULL})."));
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertFalse(this bool @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (@this)
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {nameof(AssertFalse)}({argument})."));
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
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {argument}.{nameof(AssertNotEmpty)}<IEnumerable<{typeof(T).Name}>>()."));
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertNotEquals(this string? @this, string? value, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (string.Equals(@this, value, comparisonType))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this ?? NULL}.{nameof(AssertEquals)}({value ?? NULL})."));
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertNotEquals<T>(this T? @this, T? value, IEqualityComparer<T>? comparer = null,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		comparer ??= EqualityComparer<T>.Default;

		if (comparer.Equals(@this, value))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this?.ToString() ?? NULL}.{nameof(AssertNotEquals)}<{typeof(T).Name}>({value?.ToString() ?? NULL})."));
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
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {argument}.{nameof(AssertNotEmpty)}<{typeof(T).Name}[]>()."));
	}

	/// <exception cref="ArgumentNullException"/>
	public static void AssertNotNull<T>([NotNull] this T? @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
		where T : notnull
		=> ArgumentNullException.ThrowIfNull(@this, Invariant($"{caller}: {argument}.{nameof(AssertNotNull)}<{typeof(T).Name}>()."));

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertNotSame(this object? @this, object? value, StringComparison comparison = StringComparison.OrdinalIgnoreCase,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (object.ReferenceEquals(@this, value))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this?.ToString() ?? NULL}.{nameof(AssertNotSame)}({value?.ToString() ?? NULL})."));
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertNotSame<T>(this (T?, T?) @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
		where T : notnull
	{
		if (object.ReferenceEquals(@this.Item1, @this.Item2))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this.Item1?.ToString() ?? NULL}.{nameof(AssertNotSame)}<{typeof(T).Name}>({@this.Item2?.ToString() ?? NULL})."));
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
