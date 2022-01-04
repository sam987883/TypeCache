// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Linq.Expressions;
using TypeCache.Collections.Extensions;

namespace TypeCache.Reflection.Extensions;

public readonly struct ArrayExpressionBuilder
{
	private readonly Expression _Expression;

	public ArrayExpressionBuilder(Expression expression)
	{
		this._Expression = expression;
	}

	/// <summary>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(<see langword="this"/>._Expression, <see cref="Expression"/>.Constant(<paramref name="index"/>));</c>
	/// </summary>
	public IndexExpression this[int index]
		=> Expression.ArrayAccess(this._Expression, Expression.Constant(index));

	/// <summary>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(<see langword="this"/>._Expression, <paramref name="indexes"/>.ToArray(index =&gt; (<see cref="Expression"/>)<see cref="Expression"/>.Constant(index)));</c>
	/// </summary>
	public IndexExpression this[params int[] indexes]
		=> Expression.ArrayAccess(this._Expression, indexes.ToArray(index => (Expression)Expression.Constant(index)));

	/// <summary>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(<see langword="this"/>._Expression, <see cref="Expression"/>.Constant(<paramref name="index"/>));</c>
	/// </summary>
	public IndexExpression this[long index]
		=> Expression.ArrayAccess(this._Expression, Expression.Constant(index));

	/// <summary>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(<see langword="this"/>._Expression, <paramref name="indexes"/>.ToArray(index =&gt; (<see cref="Expression"/>)<see cref="Expression"/>.Constant(index)));</c>
	/// </summary>
	public IndexExpression this[params long[] indexes]
		=> Expression.ArrayAccess(this._Expression, indexes.ToArray(index => (Expression)Expression.Constant(index)));

	/// <summary>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(<see langword="this"/>._Expression, <paramref name="index"/>);</c>
	/// </summary>
	public IndexExpression this[Expression index]
		=> Expression.ArrayAccess(this._Expression, index);

	/// <summary>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(<see langword="this"/>._Expression, <paramref name="indexes"/>);</c>
	/// </summary>
	public IndexExpression this[IEnumerable<Expression> indexes]
		=> Expression.ArrayAccess(this._Expression, indexes);

	/// <summary>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(<see langword="this"/>._Expression, <paramref name="indexes"/>);</c>
	/// </summary>
	public IndexExpression this[params Expression[] indexes]
		=> Expression.ArrayAccess(this._Expression, indexes);

	/// <summary>
	/// <c>=&gt; <see cref="Expression"/>.ArrayLength(<see langword="this"/>._Expression);</c>
	/// </summary>
	public UnaryExpression Length => Expression.ArrayLength(this._Expression);
}
