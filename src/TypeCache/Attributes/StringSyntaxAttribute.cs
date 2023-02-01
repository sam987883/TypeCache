// Copyright (c) 2021 Samuel Abraham

namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
public sealed class StringSyntaxAttribute : Attribute
{
	public const string CompositeFormat = nameof(CompositeFormat);

	public const string Regex = nameof(Regex);

	public const string DateTimeFormat = nameof(DateTimeFormat);

	public StringSyntaxAttribute(string syntax)
	{
	}

	public StringSyntaxAttribute(string syntax, params object?[] arguments)
	{
	}
}
