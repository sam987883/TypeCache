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

	public void AssertEmpty<T>(IEnumerable<T>? items
		, [CallerArgumentExpression("items")] string? argument = null
		, [CallerMemberName] string? caller = null)
	{
		if (items?.Any() is true)
			this.Fails.Add(Invariant($"FAIL: {caller} ---> {argument} is not empty."));
	}

	public void AssertEquals<T>(T value1, T value2
		, [CallerArgumentExpression("value1")] string? argument1 = null
		, [CallerArgumentExpression("value2")] string? argument2 = null
		, [CallerMemberName] string? caller = null)
		where T : struct
	{
		if (!EqualityComparer<T>.Default.Equals(value1, value2))
			this.Fails.Add(Invariant($"FAIL: {caller} ---> {argument1} ({value1}) <> {argument2} ({value2})."));
	}

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

	public void AssertEquals(string? value1, string? value2
		, StringComparison comparison = StringComparison.OrdinalIgnoreCase
		, [CallerArgumentExpression("value1")] string? argument1 = null
		, [CallerArgumentExpression("value2")] string? argument2 = null
		, [CallerMemberName] string? caller = null)
	{
		if (!comparison.ToStringComparer().Equals(value1, value2))
			this.Fails.Add($"FAIL: {caller} ({comparison.Name()}) ---> {argument1} ({value1?.ToString() ?? NULL}) <> {argument2} ({value2?.ToString() ?? NULL}).");
	}

	public void AssertNotBlank(string? value
		, [CallerArgumentExpression("value")] string? argument = null
		, [CallerMemberName] string? caller = null)
	{
		if (value.IsBlank())
			this.Fails.Add(Invariant($"FAIL: {caller} ---> {argument} is blank."));
	}

	public void AssertNotEmpty<T>(IEnumerable<T>? items
		, [CallerArgumentExpression("items")] string? argument = null
		, [CallerMemberName] string? caller = null)
	{
		if (items?.Any() is not true)
			this.Fails.Add(Invariant($"FAIL: {caller} ---> {argument} is {(items is not null ? "empty" : NULL)}."));
	}

	public void AssertNotEmpty<T>(T[]? items
		, [CallerArgumentExpression("items")] string? argument = null
		, [CallerMemberName] string? caller = null)
	{
		if (items?.Any() is not true)
			this.Fails.Add(Invariant($"FAIL: {caller} ---> {argument} is {(items is not null ? "empty" : NULL)}."));
	}

	public void AssertNotNull<T>(T? value
		, [CallerArgumentExpression("value")] string? argument = null
		, [CallerMemberName] string? caller = null)
	{
		if (value is null)
			this.Fails.Add(Invariant($"FAIL: {caller} ---> {argument} is {NULL}."));
	}

	public void AssertNotSame(object? value1, object? value2,
		[CallerArgumentExpression("value1")] string? argument = null,
		[CallerMemberName] string? caller = null)
	{
		if (object.ReferenceEquals(value1, value2))
			this.Fails.Add(Invariant($"FAIL: {caller} ---> {value1?.ToString() ?? NULL} == {value2?.ToString() ?? NULL}."));
	}

	public void IncludeError(Exception error)
	{
		if (error is ValidationException validationException)
			this.Fails.AddRange(validationException.ValidationMessages);
		else
			this.Fails.Add(error.Message);
	}
}
