// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class AssertExtensions
{
	/// <summary>
	/// <code>
	/// <see langword="if"/> (!@<paramref name="this"/>)<br/>
	/// <see langword="    throw new"/> <see cref="ArgumentOutOfRangeException"/>($"{<see langword="nameof"/>(Assert)} ({<paramref name="caller"/>}): {<paramref name="condition"/>}.", <paramref name="argument"/>);
	/// </code>
	/// </summary>
	/// <param name="this">The value to evaluate- a value of <see langword="false"/> will throw an exception.</param>
	/// <param name="argument">The name of the argument used by the exception thrown.</param>
	/// <param name="caller">The method name that called this Assert method.</param>
	/// <param name="condition">The C# expression that produced the value for @<paramref name="this"/>.</param>
	/// <exception cref="ArgumentOutOfRangeException"/>
	private static void Assert(this bool @this, string? argument, string? caller,
		[CallerArgumentExpression("this")] string? condition = null)
	{
		if (!@this)
			throw new ArgumentOutOfRangeException($"{nameof(Assert)} ({caller}): {condition}.", argument);
	}

	/// <summary>
	/// <code>
	/// <paramref name="name"/>.AssertNotBlank(<see langword="nameof"/>(<paramref name="name"/>));<br/>
	/// <br/>
	/// <see langword="if"/> (@<paramref name="this"/> <see langword="is"/> <see cref="IEquatable{T}"/> equatable)<br/>
	///	<see langword="    "/>equatable.Equals(<paramref name="value"/>).Assert(<paramref name="argument"/>, <paramref name="caller"/>);<br/>
	///	<see langword="else"/><br/>
	///	<see langword="    "/><see cref="object"/>.Equals(@<paramref name="this"/>, <paramref name="value"/>).Assert(<paramref name="argument"/>, <paramref name="caller"/>);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void Assert<T>(this T @this, T value,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
		where T : struct
	{
		if (@this is IEquatable<T> equatable)
			equatable.Equals(value).Assert(argument, caller);
		else
			object.Equals(@this, value).Assert(argument, caller);
	}

	/// <summary>
	/// <code>
	/// <paramref name="comparer"/>.AssertNotNull();<br/>
	/// <paramref name="comparer"/>.Equals(@<paramref name="this"/>, <paramref name="value"/>).Assert(<paramref name="argument"/>, <paramref name="caller"/>);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void Assert<T>(this T? @this, T? value, IEqualityComparer<T> comparer,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		comparer.AssertNotNull();
		comparer.Equals(@this, value).Assert(argument, caller);
	}

	/// <summary>
	/// <c>=&gt; <paramref name="comparison"/>.ToStringComparer().Equals(@<paramref name="this"/>, <paramref name="value"/>)).Assert(<paramref name="argument"/>, <paramref name="caller"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void Assert(this string? @this, string? value, StringComparison comparison = STRING_COMPARISON,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
		=> comparison.ToStringComparer().Equals(@this, value).Assert(argument, caller);

	/// <summary>
	/// <code>
	/// @<paramref name="this"/>.AssertNotNull(<paramref name="argument"/>, <paramref name="caller"/>);<br/>
	/// (!@<paramref name="this"/>.IsBlank()).Assert(<paramref name="argument"/>, <paramref name="caller"/>);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertNotBlank(this string? @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		@this.AssertNotNull(argument, caller);
		(!@this.IsBlank()).Assert(argument, caller);
	}

	/// <summary>
	/// <code>
	/// @<paramref name="this"/>.AssertNotNull(<paramref name="argument"/>, <paramref name="caller"/>);<br/>
	/// @<paramref name="this"/>.Any().Assert(<paramref name="argument"/>, <paramref name="caller"/>);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertNotEmpty<T>(this IEnumerable<T>? @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		@this.AssertNotNull(argument, caller);
		@this.Any().Assert(argument, caller);
	}

	/// <summary>
	/// <code>
	/// @<paramref name="this"/>.AssertNotNull(<paramref name="argument"/>, <paramref name="caller"/>);<br/>
	/// @<paramref name="this"/>.Any().Assert(<paramref name="argument"/>, <paramref name="caller"/>);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertNotEmpty<T>(this T[]? @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		@this.AssertNotNull(argument, caller);
		@this.Any().Assert(argument, caller);
	}

	/// <summary>
	/// <code>
	/// <see langword="if"/> (@<paramref name="this"/> <see langword="is null"/>)<br/>
	/// <see langword="    throw new"/> <see cref="ArgumentNullException"/>($"{<see langword="nameof"/>(Assert)} ({<paramref name="caller"/>}).", <paramref name="argument"/>);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static void AssertNotNull<T>(this T? @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (@this is null)
			throw new ArgumentNullException($"{nameof(Assert)} ({caller}).", argument);
	}

	/// <summary>
	/// <c>=&gt; (!<see cref="object"/>.ReferenceEquals(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2)).Assert(<paramref name="argument"/>, <paramref name="caller"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void AssertNotSame<T>(this (T?, T?) @this,
		[CallerArgumentExpression("this")] string? argument = null,
		[CallerMemberName] string? caller = null)
		=> (!object.ReferenceEquals(@this.Item1, @this.Item2)).Assert(argument, caller);
}
