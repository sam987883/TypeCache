// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;

namespace TypeCache.Reflection.Extensions;

public enum UnaryOperator
{
	/// <inheritdoc cref="Expression.IsTrue(Expression)"/>
	/// <remarks><c>i <see langword="is true"/></c></remarks>
	IsTrue,
	/// <inheritdoc cref="Expression.IsFalse(Expression)"/>
	/// <remarks><c>i <see langword="is false"/></c></remarks>
	IsFalse,
	/// <inheritdoc cref="Expression.PreIncrementAssign(Expression)"/>
	/// <remarks><c>++i</c></remarks>
	PreIncrement,
	/// <inheritdoc cref="Expression.Increment(Expression)"/>
	/// <remarks><c>i + 1</c></remarks>
	Increment,
	/// <inheritdoc cref="Expression.PostIncrementAssign(Expression)"/>
	/// <remarks><c>i++</c></remarks>
	PostIncrement,
	/// <inheritdoc cref="Expression.PreDecrementAssign(Expression)"/>
	/// <remarks><c>--i</c></remarks>
	PreDecrement,
	/// <inheritdoc cref="Expression.Decrement(Expression)"/>
	/// <remarks><c>i - 1</c></remarks>
	Decrement,
	/// <inheritdoc cref="Expression.PostDecrementAssign(Expression)"/>
	/// <remarks><c>i--</c></remarks>
	PostDecrement,
	/// <inheritdoc cref="Expression.Negate(Expression)"/>
	/// <remarks><c>-i</c></remarks>
	Negate,
	/// <inheritdoc cref="Expression.NegateChecked(Expression)"/>
	/// <remarks><c><see langword="checked"/>(-i)</c></remarks>
	NegateChecked,
	/// <inheritdoc cref="Expression.OnesComplement(Expression)"/>
	/// <remarks><c>~i</c></remarks>
	Complement
}
