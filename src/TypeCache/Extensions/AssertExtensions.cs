// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using static System.FormattableString;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class AssertExtensions
{
	private const string NULL = "null";

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (!<see cref="EqualityComparer{T}.Default"/>.Equals(@<paramref name="this"/>, <paramref name="value"/>)<br/>
	///	<see langword="        throw new"/> <see cref="ArgumentOutOfRangeException"/>(<paramref name="argument"/>, Invariant($"{<paramref name="caller"/>}: {@<paramref name="this"/>}.{<see langword="nameof"/>(AssertEquals)}&lt;{<see cref="TypeOf{T}.Name"/>}&gt;({<paramref name="value"/>})."));<br/>
	///	}
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertEquals<T>(this T @this, T value,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
		where T : struct
	{
		if (!EqualityComparer<T>.Default.Equals(@this, value))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this}.{nameof(AssertEquals)}<{TypeOf<T>.Name}>({value})."));
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    "/><paramref name="comparer"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="    if"/> (!<paramref name="comparer"/>.Equals(@<paramref name="this"/>, <paramref name="value"/>)<br/>
	///	<see langword="        throw new"/> <see cref="ArgumentOutOfRangeException"/>(<paramref name="argument"/>, Invariant($"{<paramref name="caller"/>}: {@<paramref name="this"/>?.ToString() ?? "null"}.{<see langword="nameof"/>(AssertEquals)}&lt;{<see cref="TypeOf{T}.Name"/>}&gt;({<paramref name="value"/>?.ToString() ?? "null"})."));
	///	}
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertEquals<T>(this T? @this, T? value, IEqualityComparer<T> comparer,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
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
	public static void AssertEquals(this string? @this, string? value, StringComparison comparison = STRING_COMPARISON,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
		=> @this.AssertEquals(value, comparison.ToStringComparer(), argument, caller);

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (@<paramref name="this"/>)<br/>
	///	<see langword="        throw new"/> <see cref="ArgumentOutOfRangeException"/>(<paramref name="argument"/>, Invariant($"{<paramref name="caller"/>}: {<see langword="nameof"/>(AssertFalse)}({<paramref name="argument"/>})."));<br/>
	///	}
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertFalse(this bool @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (@this)
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {nameof(AssertFalse)}({argument})."));
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (!<see cref="object"/>.ReferenceEquals(@<paramref name="this"/>, <paramref name="value"/>))<br/>
	///	<see langword="        throw new"/> <see cref="ArgumentOutOfRangeException"/>(<paramref name="argument"/>, Invariant($"{<paramref name="caller"/>}: {<paramref name="argument"/>}.{<see langword="nameof"/>(AssertNotSame)}()."));<br/>
	///	}
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertNotSame(this object? @this, object? value, StringComparison comparison = STRING_COMPARISON,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (object.ReferenceEquals(@this, value))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this?.ToString() ?? NULL}.{nameof(AssertNotSame)}({value?.ToString() ?? NULL})."));
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    "/>@<paramref name="this"/>.AssertNotNull(<paramref name="argument"/>, <paramref name="caller"/>);<br/>
	/// <br/>
	/// <see langword="    if"/> (@<paramref name="this"/>.IsBlank())<br/>
	///	<see langword="        throw new"/> <see cref="ArgumentOutOfRangeException"/>(<paramref name="argument"/>, Invariant($"{<paramref name="caller"/>}: {<paramref name="argument"/>}.{<see langword="nameof"/>(AssertNotBlank)}()."));<br/>
	///	}
	/// </code>
	/// </summary>
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

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    "/>@<paramref name="this"/>.AssertNotNull(<paramref name="argument"/>, <paramref name="caller"/>);<br/>
	/// <br/>
	/// <see langword="    if"/> (@<paramref name="this"/>.IsBlank())<br/>
	///	<see langword="        throw new"/> <see cref="ArgumentOutOfRangeException"/>(<paramref name="argument"/>, Invariant($"{<paramref name="caller"/>}: {<paramref name="argument"/>}.{<see langword="nameof"/>(AssertNotEmpty)}&lt;IEnumerable&lt;{<see cref="TypeOf{T}.Name"/>}&gt;&gt;()."));<br/>
	///	}
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertNotEmpty<T>([NotNull] this IEnumerable<T>? @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		@this.AssertNotNull(argument, caller);

		if (!@this.Any())
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {argument}.{nameof(AssertNotEmpty)}<IEnumerable<{TypeOf<T>.Name}>>()."));
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    "/>@<paramref name="this"/>.AssertNotNull(<paramref name="argument"/>, <paramref name="caller"/>);<br/>
	/// <br/>
	/// <see langword="    if"/> (@<paramref name="this"/>.IsBlank())<br/>
	///	<see langword="        throw new"/> <see cref="ArgumentOutOfRangeException"/>(<paramref name="argument"/>, Invariant($"{<paramref name="caller"/>}: {<paramref name="argument"/>}.{<see langword="nameof"/>(AssertNotEmpty)}&lt;{<see cref="TypeOf{T}.Name"/>[]}&gt;()."));<br/>
	///	}
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertNotEmpty<T>([NotNull] this T[]? @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		@this.AssertNotNull(argument, caller);

		if (!@this.Any())
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {argument}.{nameof(AssertNotEmpty)}<{TypeOf<T>.Name}[]>()."));
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (@<paramref name="this"/> <see langword="is null"/>)<br/>
	///	<see langword="        throw new"/> <see cref="ArgumentNullException"/>(<paramref name="argument"/>, Invariant($"{<paramref name="caller"/>}: {<paramref name="argument"/>}.{<see langword="nameof"/>(AssertNotNull)}()."));<br/>
	///	}
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static void AssertNotNull<T>([NotNull] this T? @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (@this is null)
			throw new ArgumentNullException(argument, Invariant($"{caller}: {argument}.{nameof(AssertNotNull)}<{TypeOf<T>.Name}>()."));
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (<see cref="object"/>.ReferenceEquals(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2))<br/>
	///	<see langword="        throw new"/> <see cref="ArgumentOutOfRangeException"/>(<paramref name="argument"/>, Invariant($"{<paramref name="caller"/>}: {@<paramref name="this"/>.Item1?.ToString() ?? "null"}.{<see langword="nameof"/>(AssertNotSame)}&lt;{<see cref="TypeOf{T}.Name"/>}&gt;({@<paramref name="this"/>.Item2?.ToString() ?? "null"})"));<br/>
	///	}
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertNotSame<T>(this (T?, T?) @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (object.ReferenceEquals(@this.Item1, @this.Item2))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this.Item1?.ToString() ?? NULL}.{nameof(AssertNotSame)}<{TypeOf<T>.Name}>({@this.Item2?.ToString() ?? NULL})."));
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (!<see cref="object"/>.ReferenceEquals(@<paramref name="this"/>, <paramref name="value"/>))<br/>
	///	<see langword="        throw new"/> <see cref="ArgumentOutOfRangeException"/>(<paramref name="argument"/>, Invariant($"{<paramref name="caller"/>}: {<paramref name="argument"/>}.{<see langword="nameof"/>(AssertNotSame)}()."));<br/>
	///	}
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertSame(this object? @this, object? value,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (object.ReferenceEquals(@this, value))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {@this?.ToString() ?? NULL}.{nameof(AssertNotSame)}({value?.ToString() ?? NULL})."));
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (!@<paramref name="this"/>)<br/>
	///	<see langword="        throw new"/> <see cref="ArgumentOutOfRangeException"/>(<paramref name="argument"/>, Invariant($"{<paramref name="caller"/>}: {<see langword="nameof"/>(AssertTrue)}({<paramref name="argument"/>})."));<br/>
	///	}
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertTrue(this bool @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (!@this)
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {nameof(AssertTrue)}({argument})."));
	}
}
