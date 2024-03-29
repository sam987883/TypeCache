﻿// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;

namespace TypeCache.Extensions;

public readonly struct ArrayExpressionBuilder
{
	private readonly Expression _Expression;

	public ArrayExpressionBuilder(Expression expression)
	{
		this._Expression = expression;
	}

	/// <inheritdoc cref="Expression.ArrayAccess(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(<see langword="this"/>._Expression, <see cref="Expression"/>.Constant(<paramref name="index"/>));</c>
	/// </remarks>
	[DebuggerHidden]
	public IndexExpression this[int index]
		=> Expression.ArrayAccess(this._Expression, Expression.Constant(index));

	/// <inheritdoc cref="Expression.ArrayAccess(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(<see langword="this"/>._Expression, <paramref name="indexes"/>.Select(index =&gt; <see cref="Expression"/>.Constant(index)));</c>
	/// </remarks>
	[DebuggerHidden]
	public IndexExpression this[int[] indexes]
		=> Expression.ArrayAccess(this._Expression, indexes.Select(index => Expression.Constant(index)));

	/// <inheritdoc cref="Expression.ArrayAccess(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(<see langword="this"/>._Expression, <see cref="Expression"/>.Constant(<paramref name="index"/>));</c>
	/// </remarks>
	[DebuggerHidden]
	public IndexExpression this[long index]
		=> Expression.ArrayAccess(this._Expression, Expression.Constant(index));

	/// <inheritdoc cref="Expression.ArrayAccess(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(<see langword="this"/>._Expression, <paramref name="indexes"/>.Select(index =&gt; <see cref="Expression"/>.Constant(index)));</c>
	/// </remarks>
	[DebuggerHidden]
	public IndexExpression this[long[] indexes]
		=> Expression.ArrayAccess(this._Expression, indexes.Select(index => Expression.Constant(index)));

	/// <inheritdoc cref="Expression.ArrayAccess(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(<see langword="this"/>._Expression, <paramref name="index"/>);</c>
	/// </remarks>
	[DebuggerHidden]
	public IndexExpression this[Expression index]
		=> Expression.ArrayAccess(this._Expression, index);

	/// <inheritdoc cref="Expression.ArrayAccess(Expression, IEnumerable{Expression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(<see langword="this"/>._Expression, <paramref name="indexes"/>);</c>
	/// </remarks>
	[DebuggerHidden]
	public IndexExpression this[IEnumerable<Expression> indexes]
		=> Expression.ArrayAccess(this._Expression, indexes);

	/// <inheritdoc cref="Expression.ArrayAccess(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(<see langword="this"/>._Expression, <paramref name="indexes"/>);</c>
	/// </remarks>
	[DebuggerHidden]
	public IndexExpression this[Expression[] indexes]
		=> Expression.ArrayAccess(this._Expression, indexes);

	/// <inheritdoc cref="Expression.ArrayLength(Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayLength(<see langword="this"/>._Expression);</c>
	/// </remarks>
	[DebuggerHidden]
	public UnaryExpression Length => Expression.ArrayLength(this._Expression);
}
