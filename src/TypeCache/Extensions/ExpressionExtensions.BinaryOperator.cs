// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;

namespace TypeCache.Extensions;

public enum BinaryOperator
{
	/// <inheritdoc cref="Expression.Add(Expression, Expression)"/>
	/// <remarks><c>a + b</c></remarks>
	Add,
	/// <inheritdoc cref="Expression.AddAssign(Expression, Expression)"/>
	/// <remarks><c>a += b</c></remarks>
	AddAssign,
	/// <inheritdoc cref="Expression.AddChecked(Expression, Expression)"/>
	/// <remarks><c><see langword="checked"/>(a + b)</c></remarks>
	AddChecked,
	/// <inheritdoc cref="Expression.AddAssignChecked(Expression, Expression)"/>
	/// <remarks><c><see langword="checked"/>(a += b)</c></remarks>
	AddAssignChecked,
	/// <inheritdoc cref="Expression.Divide(Expression, Expression)"/>
	/// <remarks><c>a / b</c></remarks>
	Divide,
	/// <inheritdoc cref="Expression.DivideAssign(Expression, Expression)"/>
	/// <remarks><c>a /= b</c></remarks>
	DivideAssign,
	/// <inheritdoc cref="Expression.Modulo(Expression, Expression)"/>
	/// <remarks><c>a % b</c></remarks>
	Modulo,
	/// <inheritdoc cref="Expression.ModuloAssign(Expression, Expression)"/>
	/// <remarks><c>a %= b</c></remarks>
	ModuloAssign,
	/// <inheritdoc cref="Expression.Multiply(Expression, Expression)"/>
	/// <remarks><c>a * b</c></remarks>
	Multiply,
	/// <inheritdoc cref="Expression.MultiplyAssign(Expression, Expression)"/>
	/// <remarks><c>a *= b</c></remarks>
	MultiplyAssign,
	/// <inheritdoc cref="Expression.MultiplyChecked(Expression, Expression)"/>
	/// <remarks><c><see langword="checked"/>(a * b)</c></remarks>
	MultiplyChecked,
	/// <inheritdoc cref="Expression.MultiplyAssignChecked(Expression, Expression)"/>
	/// <remarks><c><see langword="checked"/>(a *= b)</c></remarks>
	MultiplyAssignChecked,
	/// <inheritdoc cref="Expression.Power(Expression, Expression)"/>
	/// <remarks><c>a ^ b</c></remarks>
	Power,
	/// <inheritdoc cref="Expression.PowerAssign(Expression, Expression)"/>
	/// <remarks><c>a ^= b</c></remarks>
	PowerAssign,
	/// <inheritdoc cref="Expression.Subtract(Expression, Expression)"/>
	/// <remarks><c>a - b</c></remarks>
	Subtract,
	/// <inheritdoc cref="Expression.SubtractAssign(Expression, Expression)"/>
	/// <remarks><c>a -= b</c></remarks>
	SubtractAssign,
	/// <inheritdoc cref="Expression.SubtractChecked(Expression, Expression)"/>
	/// <remarks><c><see langword="checked"/>(a - b)</c></remarks>
	SubtractChecked,
	/// <inheritdoc cref="Expression.SubtractAssignChecked(Expression, Expression)"/>
	/// <remarks><c><see langword="checked"/>(a -= b)</c></remarks>
	SubtractAssignChecked,
	/// <inheritdoc cref="Expression.And(Expression, Expression)"/>
	/// <remarks><c>a &amp; b</c></remarks>
	And,
	/// <inheritdoc cref="Expression.AndAssign(Expression, Expression)"/>
	/// <remarks><c>a &amp;= b</c></remarks>
	AndAssign,
	/// <inheritdoc cref="Expression.Or(Expression, Expression)"/>
	/// <remarks><c>a | b</c></remarks>
	Or,
	/// <inheritdoc cref="Expression.OrAssign(Expression, Expression)"/>
	/// <remarks><c>a |= b</c></remarks>
	OrAssign,
	/// <inheritdoc cref="Expression.ExclusiveOr(Expression, Expression)"/>
	/// <remarks><c>a ^ b</c></remarks>
	ExclusiveOr,
	/// <inheritdoc cref="Expression.ExclusiveOrAssign(Expression, Expression)"/>
	/// <remarks><c>a ^= b</c></remarks>
	ExclusiveOrAssign,
	/// <inheritdoc cref="Expression.LeftShift(Expression, Expression)"/>
	/// <remarks><c>a &lt;&lt; 6</c></remarks>
	LeftShift,
	/// <inheritdoc cref="Expression.LeftShiftAssign(Expression, Expression)"/>
	/// <remarks><c>a &lt;&lt;= 6</c></remarks>
	LeftShiftAssign,
	/// <inheritdoc cref="Expression.RightShift(Expression, Expression)"/>
	/// <remarks><c>a &gt;&gt; 6</c></remarks>
	RightShift,
	/// <inheritdoc cref="Expression.RightShiftAssign(Expression, Expression)"/>
	/// <remarks><c>a &gt;&gt;= 6</c></remarks>
	RightShiftAssign,
	/// <inheritdoc cref="Expression.Equal(Expression, Expression)"/>
	/// <remarks><c>a == b</c></remarks>
	Equal,
	/// <inheritdoc cref="Expression.ReferenceEqual(Expression, Expression)"/>
	/// <remarks><c>a != b</c></remarks>
	NotEqual,
	/// <inheritdoc cref="Expression.ReferenceNotEqual(Expression, Expression)"/>
	/// <remarks><c>a is b</c></remarks>
	ReferenceEqual,
	/// <inheritdoc cref="Expression.NotEqual(Expression, Expression)"/>
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
	LessThanOrEqual
}
