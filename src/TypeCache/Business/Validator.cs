// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using static System.FormattableString;
using static TypeCache.Default;

namespace TypeCache.Business;

public readonly struct Validator
{
	private const string NULL = "null";

	private readonly List<string> _Fails;

	public Validator()
	{
		this._Fails = new();
	}

	public IReadOnlyCollection<string> Fails => this._Fails;

	public bool Success => !this._Fails.Any();

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (<paramref name="items"/>.Any())<br/>
	///	<see langword="        this"/>._Fails.Add($"FAIL: {<paramref name="caller"/>} ---&gt; {<paramref name="argument"/>} is not empty.");<br/>
	///	}
	/// </code>
	/// </summary>
	public void AssertEmpty<T>(IEnumerable<T>? items
		, [CallerArgumentExpression("items")] string? argument = null
		, [CallerMemberName] string? caller = null)
	{
		if (items.Any())
			this._Fails.Add(Invariant($"FAIL: {caller} ---> {argument} is not empty."));
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (!<see cref="EqualityComparer{T}.Default"/>.Equals(<paramref name="value1"/>, <paramref name="value2"/>))<br/>
	///	<see langword="        this"/>._Fails.Add(Invariant($"FAIL: {<paramref name="caller"/>} ---&gt; {<paramref name="argument1"/>} ({<paramref name="value1"/>}) &lt;&gt; {<paramref name="argument2"/>} ({<paramref name="value2"/>})."));<br/>
	///	}
	/// </code>
	/// </summary>
	public void AssertEquals<T>(T value1, T value2
		, [CallerArgumentExpression("value1")] string? argument1 = null
		, [CallerArgumentExpression("value2")] string? argument2 = null
		, [CallerMemberName] string? caller = null)
		where T : struct
	{
		if (!EqualityComparer<T>.Default.Equals(value1, value2))
			this._Fails.Add(Invariant($"FAIL: {caller} ---> {argument1} ({value1}) <> {argument2} ({value2})."));
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    var"/> message = <paramref name="comparer"/> <see langword="switch"/><br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        null"/> =&gt; Invariant($"FAIL: {<paramref name="caller"/>} - {<see langword="nameof"/>(<paramref name="comparer"/>)} is null."),<br/>
	/// <see langword="        "/>_ <see langword="when"/> !<paramref name="comparer"/>.Equals(<paramref name="value1"/>, <paramref name="value2"/>) =&gt; Invariant($"FAIL: {<paramref name="caller"/>} ---&gt; {<paramref name="argument1"/>} ({<paramref name="value1"/>}) &lt;&gt; {<paramref name="argument2"/>} ({<paramref name="value2"/>})."),<br/>
	/// <see langword="        "/>_ =&gt; <see langword="null"/><br/>
	/// <see langword="    "/>};<br/>
	/// <see langword="    if"/> (message <see langword="is not null"/>)<br/>
	///	<see langword="        this"/>._Fails.Add(message);
	///	}
	/// </code>
	/// </summary>
	public void AssertEquals<T>(T value1, T value2
		, IEqualityComparer<T> comparer
		, [CallerArgumentExpression("value1")] string? argument1 = null
		, [CallerArgumentExpression("value2")] string? argument2 = null
		, [CallerMemberName] string? caller = null)
	{
		var message = comparer switch
		{
			null => Invariant($"FAIL: {caller} - {nameof(comparer)} is null."),
			_ when !comparer.Equals(value1, value2) => Invariant($"FAIL: {caller} ---> {argument1} ({value1}) <> {argument2} ({value2})."),
			_ => null
		};
		if (message is not null)
			this._Fails.Add(message);
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (!<paramref name="comparison"/>.ToStringComparer().Equals(<paramref name="value1"/>, <paramref name="value2"/>))<br/>
	///	<see langword="        this"/>._Fails.Add($"FAIL: {caller} ({<paramref name="comparison"/>.Name()}) ---&gt; {<paramref name="argument1"/>} ({<paramref name="value1"/>?.ToString() ?? "null"}) &lt;&gt; {<paramref name="argument2"/>} ({<paramref name="value2"/>?.ToString() ?? "null"}).");<br/>
	///	}
	/// </code>
	/// </summary>
	public void AssertEquals(string? value1, string? value2
		, StringComparison comparison = STRING_COMPARISON
		, [CallerArgumentExpression("value1")] string? argument1 = null
		, [CallerArgumentExpression("value2")] string? argument2 = null
		, [CallerMemberName] string? caller = null)
	{
		if (!comparison.ToStringComparer().Equals(value1, value2))
			this._Fails.Add($"FAIL: {caller} ({comparison.Name()}) ---> {argument1} ({value1?.ToString() ?? NULL}) <> {argument2} ({value2?.ToString() ?? NULL}).");
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (<paramref name="value"/>.IsBlank())<br/>
	///	<see langword="        this"/>._Fails.Add($"FAIL: {<paramref name="caller"/>} ---&gt; {<paramref name="argument"/>} is blank.");<br/>
	///	}
	/// </code>
	/// </summary>
	public void AssertNotBlank(string? value
		, [CallerArgumentExpression("value")] string? argument = null
		, [CallerMemberName] string? caller = null)
	{
		if (value.IsBlank())
			this._Fails.Add(Invariant($"FAIL: {caller} ---> {argument} is blank."));
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (!<paramref name="items"/>.Any())<br/>
	///	<see langword="        this"/>._Fails.Add($"FAIL: {<paramref name="caller"/>} ---&gt; {<paramref name="argument"/>} is {(<paramref name="items"/> <see langword="is not null"/> ? "empty" : "null")}.");<br/>
	///	}
	/// </code>
	/// </summary>
	public void AssertNotEmpty<T>(IEnumerable<T>? items
		, [CallerArgumentExpression("items")] string? argument = null
		, [CallerMemberName] string? caller = null)
	{
		if (!items.Any())
			this._Fails.Add(Invariant($"FAIL: {caller} ---> {argument} is {(items is not null ? "empty" : NULL)}."));
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (!<paramref name="items"/>.Any())<br/>
	///	<see langword="        this"/>._Fails.Add($"FAIL: {<paramref name="caller"/>} ---&gt; {<paramref name="argument"/>} is {(<paramref name="items"/> <see langword="is not null"/> ? "empty" : "null")}.");<br/>
	///	}
	/// </code>
	/// </summary>
	public void AssertNotEmpty<T>(T[]? items
		, [CallerArgumentExpression("items")] string? argument = null
		, [CallerMemberName] string? caller = null)
	{
		if (!items.Any())
			this._Fails.Add(Invariant($"FAIL: {caller} ---> {argument} is {(items is not null ? "empty" : NULL)}."));
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (@<paramref name="this"/> <see langword="is null"/>)<br/>
	///	<see langword="        this"/>._Fails.Add($"FAIL: {<paramref name="caller"/>} ---&gt; {<paramref name="argument"/>} is null.");<br/>
	///	}
	/// </code>
	/// </summary>
	public void AssertNotNull<T>(T? value
		, [CallerArgumentExpression("value")] string? argument = null
		, [CallerMemberName] string? caller = null)
	{
		if (value is null)
			this._Fails.Add(Invariant($"FAIL: {caller} ---> {argument} is {NULL}."));
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (<see cref="object"/>.ReferenceEquals(<paramref name="value1"/>, <paramref name="value2"/>))<br/>
	///	<see langword="        throw new"/> <see cref="ArgumentOutOfRangeException"/>(<paramref name="argument"/>, Invariant($"{<paramref name="caller"/>}: {<paramref name="value1"/>?.ToString() ?? "null"}.{<see langword="nameof"/>(AssertNotSame)}({<paramref name="value2"/>?.ToString() ?? "null"})."));<br/>
	///	}
	/// </code>
	/// </summary>
	public void AssertNotSame(object? value1, object? value2,
		[CallerArgumentExpression("value1")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (object.ReferenceEquals(value1, value2))
			throw new ArgumentOutOfRangeException(argument, Invariant($"{caller}: {value1?.ToString() ?? NULL}.{nameof(AssertNotSame)}({value2?.ToString() ?? NULL})."));
	}
}
