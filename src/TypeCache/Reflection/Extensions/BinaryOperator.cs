// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Reflection.Extensions;

public enum BinaryOperator
{
	/// <summary>
	/// <c>a + b</c>
	/// </summary>
	Add,
	/// <summary>
	/// <c>checked(a + b)</c>
	/// </summary>
	AddChecked,
	/// <summary>
	/// <c>a / b</c>
	/// </summary>
	Divide,
	/// <summary>
	/// <c>a % 6</c>
	/// </summary>
	Modulus,
	/// <summary>
	/// <c>a * b</c>
	/// </summary>
	Multiply,
	/// <summary>
	/// <c>checked(a * b)</c>
	/// </summary>
	MultiplyChecked,
	/// <summary>
	/// <c>a ^ b</c>
	/// </summary>
	Power,
	/// <summary>
	/// <c>a - b</c>
	/// </summary>
	Subtract,
	/// <summary>
	/// <c>checked(a - b)</c>
	/// </summary>
	SubtractChecked,
	/// <summary>
	/// <c>a &amp; b</c>
	/// </summary>
	And,
	/// <summary>
	/// <c>a | b</c>
	/// </summary>
	Or,
	/// <summary>
	/// <c>a ^ b</c>
	/// </summary>
	ExclusiveOr,
	/// <summary>
	/// <c>a &lt;&lt; 6</c>
	/// </summary>
	LeftShift,
	/// <summary>
	/// <c>a &gt;&gt; 6</c>
	/// </summary>
	RightShift,
	/// <summary>
	/// <c>a == b</c>
	/// </summary>
	EqualTo,
	/// <summary>
	/// <c>a === b</c>
	/// </summary>
	ReferenceEqualTo,
	/// <summary>
	/// <c>a != b</c>
	/// </summary>
	NotEqualTo,
	/// <summary>
	/// <c>a !== b</c>
	/// </summary>
	ReferenceNotEqualTo,
	/// <summary>
	/// <c>a &gt; b</c>
	/// </summary>
	GreaterThan,
	/// <summary>
	/// <c>a &gt;= b</c>
	/// </summary>
	GreaterThanOrEqualTo,
	/// <summary>
	/// <c>a &lt; b</c>
	/// </summary>
	LessThan,
	/// <summary>
	/// <c>a &lt;= b</c>
	/// </summary>
	LessThanOrEqualTo,
}
