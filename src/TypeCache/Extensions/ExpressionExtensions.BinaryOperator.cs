// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;

namespace TypeCache.Extensions;

public enum BinaryOperator
{
	/// <inheritdoc cref="Expression.Add(Expression, Expression)"/>
	/// <remarks><c>a + b</c></remarks>
	Add,
	/// <inheritdoc cref="Expression.AddChecked(Expression, Expression)"/>
	/// <remarks><c><see langword="checked"/>(a + b)</c></remarks>
	AddChecked,
	/// <inheritdoc cref="Expression.Divide(Expression, Expression)"/>
	/// <remarks><c>a / b</c></remarks>
	Divide,
	/// <inheritdoc cref="Expression.Modulo(Expression, Expression)"/>
	/// <remarks><c>a % b</c></remarks>
	Modulus,
	/// <inheritdoc cref="Expression.Multiply(Expression, Expression)"/>
	/// <remarks><c>a * b</c></remarks>
	Multiply,
	/// <inheritdoc cref="Expression.MultiplyChecked(Expression, Expression)"/>
	/// <remarks><c><see langword="checked"/>(a * b)</c></remarks>
	MultiplyChecked,
	/// <inheritdoc cref="Expression.Power(Expression, Expression)"/>
	/// <remarks><c>a ^ b</c></remarks>
	Power,
	/// <inheritdoc cref="Expression.Subtract(Expression, Expression)"/>
	/// <remarks><c>a - b</c></remarks>
	Subtract,
	/// <inheritdoc cref="Expression.SubtractChecked(Expression, Expression)"/>
	/// <remarks><c><see langword="checked"/>(a - b)</c></remarks>
	SubtractChecked,
	/// <inheritdoc cref="Expression.And(Expression, Expression)"/>
	/// <remarks><c>a &amp; b</c></remarks>
	And,
	/// <inheritdoc cref="Expression.Or(Expression, Expression)"/>
	/// <remarks><c>a | b</c></remarks>
	Or,
	/// <inheritdoc cref="Expression.ExclusiveOr(Expression, Expression)"/>
	/// <remarks><c>a ^ b</c></remarks>
	ExclusiveOr,
	/// <inheritdoc cref="Expression.LeftShift(Expression, Expression)"/>
	/// <remarks><c>a &lt;&lt; 6</c></remarks>
	LeftShift,
	/// <inheritdoc cref="Expression.RightShift(Expression, Expression)"/>
	/// <remarks><c>a &gt;&gt; 6</c></remarks>
	RightShift,
	/// <inheritdoc cref="Expression.Equal(Expression, Expression)"/>
	/// <remarks><c>a == b</c></remarks>
	Equal,
	/// <inheritdoc cref="Expression.ReferenceEqual(Expression, Expression)"/>
	/// <remarks><c>a is b</c></remarks>
	ReferenceEqual,
	/// <inheritdoc cref="Expression.NotEqual(Expression, Expression)"/>
	/// <remarks><c>a != b</c></remarks>
	NotEqual,
	/// <inheritdoc cref="Expression.ReferenceNotEqual(Expression, Expression)"/>
	/// <remarks><c>a is not b</c></remarks>
	ReferenceNotEqual,
	/// <inheritdoc cref="Expression.GreaterThan(Expression, Expression)"/>
	/// <remarks><c>a &gt; b</c></remarks>
	GreaterThan,
	/// <inheritdoc cref="Expression.GreaterThanOrEqual(Expression, Expression)"/>
	/// <remarks><c>a &gt;= b</c></remarks>
	GreaterThanOrEqual,
	/// <inheritdoc cref="Expression.LessThan(Expression, Expression)"/>
	/// <remarks><c>a &lt; b</c></remarks>
	LessThan,
	/// <inheritdoc cref="Expression.LessThanOrEqual(Expression, Expression)"/>
	/// <remarks><c>a &lt;= b</c></remarks>
	LessThanOrEqual,
}
