// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;

namespace TypeCache.Extensions;

public static class ThrowIfExtensions
{
	private const string NULL = "null";

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfNotEqual<T>(this T? @this, T? value, IEqualityComparer<T>? comparer = null,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		comparer ??= EqualityComparer<T>.Default;

		if (!comparer.Equals(@this, value))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this?.ToString() ?? NULL}.{nameof(ThrowIfNotEqual)}<{typeof(T).Name}>({argument})."));
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfNotEqualIgnoreCase(this string? @this, string? value,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (!string.Equals(@this, value, StringComparison.OrdinalIgnoreCase))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this ?? NULL}.{nameof(ThrowIfNotEqualIgnoreCase)}({argument})."));
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfBlank([NotNull] this string? @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (@this.IsBlank())
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {argument}.{nameof(ThrowIfBlank)}()."));
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfEmpty<T>([NotNull] this IEnumerable<T> @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
		where T : notnull
	{
		if (!@this.Any())
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {argument}.{nameof(ThrowIfEmpty)}<IEnumerable<{typeof(T).Name}>>()."));
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfEmpty<T>([NotNull] this T[] @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
		where T : notnull
	{
		if (@this.Length is 0)
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {argument}.{nameof(ThrowIfEmpty)}<{typeof(T).Name}[]>()."));
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfEqual<T>(this T? @this, T? value, IEqualityComparer<T>? comparer = null,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		comparer ??= EqualityComparer<T>.Default;

		if (comparer.Equals(@this, value))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this?.ToString() ?? NULL}.{nameof(ThrowIfEqual)}<{typeof(T).Name}>({argument})."));
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfEqualIgnoreCase(this string? @this, string? value,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (string.Equals(@this, value, StringComparison.OrdinalIgnoreCase))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this ?? NULL}.{nameof(ThrowIfEqualIgnoreCase)}({argument})."));
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfFalse(this bool @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (!@this)
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {nameof(ThrowIfFalse)}({argument})."));
	}

	/// <exception cref="ArgumentNullException"/>
	public static void ThrowIfNull<T>([NotNull] this T? @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
		=> ArgumentNullException.ThrowIfNull(@this, Invariant($"{caller}: {argument}.{nameof(ThrowIfNull)}<{typeof(T).Name}>()."));

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfSame<T>(this T? @this, T? value, StringComparison comparison = StringComparison.OrdinalIgnoreCase,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (object.ReferenceEquals(@this, value))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this?.ToString() ?? NULL}.{nameof(ThrowIfSame)}({value?.ToString() ?? NULL})."));
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfSame<T>(this (T?, T?) @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
		where T : notnull
	{
		if (object.ReferenceEquals(@this.Item1, @this.Item2))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this.Item1?.ToString() ?? NULL}.{nameof(ThrowIfSame)}<{typeof(T).Name}>({@this.Item2?.ToString() ?? NULL})."));
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfNotSame(this object? @this, object? value,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (object.ReferenceEquals(@this, value))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this?.ToString() ?? NULL}.{nameof(ThrowIfNotSame)}({value?.ToString() ?? NULL})."));
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ThrowIfTrue(this bool @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (@this)
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {nameof(ThrowIfTrue)}({argument})."));
	}
}
