// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Mediation;

public readonly struct Validator
{
	private const string NULL = "null";

	public Validator()
	{
		this.Fails = new();
	}

	public List<string> Fails { get; }

	public bool Success => !this.Fails.Any();

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (<paramref name="items"/>?.Any() <see langword="is true"/>)<br/>
	///	<see langword="        this"/>.Fails.Add($"FAIL: {<paramref name="caller"/>} ---&gt; {<paramref name="argument"/>} is not empty.");<br/>
	///	}
	/// </code>
	/// </summary>
	public void AssertEmpty<T>(IEnumerable<T>? items
		, [CallerArgumentExpression("items")] string? argument = null
		, [CallerMemberName] string? caller = null)
	{
		if (items?.Any() is true)
			this.Fails.Add(Invariant($"FAIL: {caller} ---> {argument} is not empty."));
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (!<see cref="EqualityComparer{T}.Default"/>.Equals(<paramref name="value1"/>, <paramref name="value2"/>))<br/>
	///	<see langword="        this"/>.Fails.Add(Invariant($"FAIL: {<paramref name="caller"/>} ---&gt; {<paramref name="argument1"/>} ({<paramref name="value1"/>}) &lt;&gt; {<paramref name="argument2"/>} ({<paramref name="value2"/>})."));<br/>
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
			this.Fails.Add(Invariant($"FAIL: {caller} ---> {argument1} ({value1}) <> {argument2} ({value2})."));
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
	///	<see langword="        this"/>.Fails.Add(message);
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
			this.Fails.Add(message);
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (!<paramref name="comparison"/>.ToStringComparer().Equals(<paramref name="value1"/>, <paramref name="value2"/>))<br/>
	///	<see langword="        this"/>.Fails.Add($"FAIL: {caller} ({<paramref name="comparison"/>.Name()}) ---&gt; {<paramref name="argument1"/>} ({<paramref name="value1"/>?.ToString() ?? "null"}) &lt;&gt; {<paramref name="argument2"/>} ({<paramref name="value2"/>?.ToString() ?? "null"}).");<br/>
	///	}
	/// </code>
	/// </summary>
	public void AssertEquals(string? value1, string? value2
		, StringComparison comparison = StringComparison.OrdinalIgnoreCase
		, [CallerArgumentExpression("value1")] string? argument1 = null
		, [CallerArgumentExpression("value2")] string? argument2 = null
		, [CallerMemberName] string? caller = null)
	{
		if (!comparison.ToStringComparer().Equals(value1, value2))
			this.Fails.Add($"FAIL: {caller} ({comparison.Name()}) ---> {argument1} ({value1?.ToString() ?? NULL}) <> {argument2} ({value2?.ToString() ?? NULL}).");
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (<paramref name="value"/>.IsBlank())<br/>
	///	<see langword="        this"/>.Fails.Add($"FAIL: {<paramref name="caller"/>} ---&gt; {<paramref name="argument"/>} is blank.");<br/>
	///	}
	/// </code>
	/// </summary>
	public void AssertNotBlank(string? value
		, [CallerArgumentExpression("value")] string? argument = null
		, [CallerMemberName] string? caller = null)
	{
		if (value.IsBlank())
			this.Fails.Add(Invariant($"FAIL: {caller} ---> {argument} is blank."));
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (<paramref name="items"/>?.Any() <see langword="is not true"/>)<br/>
	///	<see langword="        this"/>.Fails.Add($"FAIL: {<paramref name="caller"/>} ---&gt; {<paramref name="argument"/>} is {(<paramref name="items"/> <see langword="is not null"/> ? "empty" : "null")}.");<br/>
	///	}
	/// </code>
	/// </summary>
	public void AssertNotEmpty<T>(IEnumerable<T>? items
		, [CallerArgumentExpression("items")] string? argument = null
		, [CallerMemberName] string? caller = null)
	{
		if (items?.Any() is not true)
			this.Fails.Add(Invariant($"FAIL: {caller} ---> {argument} is {(items is not null ? "empty" : NULL)}."));
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (<paramref name="items"/>?.Any() <see langword="is not true"/>)<br/>
	///	<see langword="        this"/>.Fails.Add($"FAIL: {<paramref name="caller"/>} ---&gt; {<paramref name="argument"/>} is {(<paramref name="items"/> <see langword="is not null"/> ? "empty" : "null")}.");<br/>
	///	}
	/// </code>
	/// </summary>
	public void AssertNotEmpty<T>(T[]? items
		, [CallerArgumentExpression("items")] string? argument = null
		, [CallerMemberName] string? caller = null)
	{
		if (items?.Any() is not true)
			this.Fails.Add(Invariant($"FAIL: {caller} ---> {argument} is {(items is not null ? "empty" : NULL)}."));
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (@<paramref name="this"/> <see langword="is null"/>)<br/>
	///	<see langword="        this"/>.Fails.Add($"FAIL: {<paramref name="caller"/>} ---&gt; {<paramref name="argument"/>} is null.");<br/>
	///	}
	/// </code>
	/// </summary>
	public void AssertNotNull<T>(T? value
		, [CallerArgumentExpression("value")] string? argument = null
		, [CallerMemberName] string? caller = null)
	{
		if (value is null)
			this.Fails.Add(Invariant($"FAIL: {caller} ---> {argument} is {NULL}."));
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (<see cref="object"/>.ReferenceEquals(<paramref name="value1"/>, <paramref name="value2"/>))<br/>
	///	<see langword="        this"/>.Fails.Add($"FAIL: {<paramref name="caller"/>} ---&gt; {<paramref name="value1"/>?.ToString() ?? NULL} == {<paramref name="value2"/>?.ToString() ?? NULL}.");<br/>
	///	}
	/// </code>
	/// </summary>
	public void AssertNotSame(object? value1, object? value2,
		[CallerArgumentExpression("value1")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (object.ReferenceEquals(value1, value2))
			this.Fails.Add(Invariant($"FAIL: {caller} ---> {value1?.ToString() ?? NULL} == {value2?.ToString() ?? NULL}."));
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (<paramref name="error"/> <see langword="is"/> <see cref="ValidationException"/> validationException)<br/>
	///	<see langword="        this"/>.Fails.AddRange(validationException.ValidationMessages);<br/>
	/// <see langword="    else"/><br/>
	///	<see langword="        this"/>.Fails.Add(<paramref name="error"/>.Message);<br/>
	///	}
	/// </code>
	/// </summary>
	public void IncludeError(Exception error)
	{
		if (error is ValidationException validationException)
			this.Fails.AddRange(validationException.ValidationMessages);
		else
			this.Fails.Add(error.Message);
	}
}
