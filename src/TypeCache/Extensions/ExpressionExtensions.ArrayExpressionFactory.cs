// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;

namespace TypeCache.Extensions;

public readonly struct ArrayExpressionFactory(Expression expression)
{
	/// <inheritdoc cref="Expression.ArrayAccess(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(expression, <see cref="Expression"/>.Constant(<paramref name="index"/>));</c>
	/// </remarks>
	[DebuggerHidden]
	public IndexExpression this[int index]
		=> Expression.ArrayAccess(expression, Expression.Constant(index));

	/// <inheritdoc cref="Expression.ArrayAccess(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(expression, <paramref name="indexes"/>.Select(index =&gt; <see cref="Expression"/>.Constant(index)));</c>
	/// </remarks>
	[DebuggerHidden]
	public IndexExpression this[int[] indexes]
		=> Expression.ArrayAccess(expression, indexes.Select(index => Expression.Constant(index)));

	/// <inheritdoc cref="Expression.ArrayAccess(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(expression, <see cref="Expression"/>.Constant(<paramref name="index"/>));</c>
	/// </remarks>
	[DebuggerHidden]
	public IndexExpression this[long index]
		=> Expression.ArrayAccess(expression, Expression.Constant(index));

	/// <inheritdoc cref="Expression.ArrayAccess(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(expression, <paramref name="indexes"/>.Select(index =&gt; <see cref="Expression"/>.Constant(index)));</c>
	/// </remarks>
	[DebuggerHidden]
	public IndexExpression this[long[] indexes]
		=> Expression.ArrayAccess(expression, indexes.Select(index => Expression.Constant(index)));

	/// <inheritdoc cref="Expression.ArrayAccess(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(expression, <paramref name="index"/>);</c>
	/// </remarks>
	[DebuggerHidden]
	public IndexExpression this[Expression index]
		=> Expression.ArrayAccess(expression, index);

	/// <inheritdoc cref="Expression.ArrayAccess(Expression, IEnumerable{Expression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(expression, <paramref name="indexes"/>);</c>
	/// </remarks>
	[DebuggerHidden]
	public IndexExpression this[IEnumerable<Expression> indexes]
		=> Expression.ArrayAccess(expression, indexes);

	/// <inheritdoc cref="Expression.ArrayAccess(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayAccess(expression, <paramref name="indexes"/>);</c>
	/// </remarks>
	[DebuggerHidden]
	public IndexExpression this[Expression[] indexes]
		=> Expression.ArrayAccess(expression, indexes);

	/// <inheritdoc cref="Expression.ArrayLength(Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayLength(expression);</c>
	/// </remarks>
	[DebuggerHidden]
	public UnaryExpression Length => Expression.ArrayLength(expression);

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <see cref="Expression"/>.Constant(<paramref name="index"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public BinaryExpression Index(int index)
		=> Expression.ArrayIndex(expression, Expression.Constant(index));

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <paramref name="indexes"/>.Select(index =&gt; (<see cref="Expression"/>)<see cref="Expression"/>.Constant(index)));</c>
	/// </remarks>
	public MethodCallExpression Index(int[] indexes)
		=> Expression.ArrayIndex(expression, indexes.Select(index => (Expression)Expression.Constant(index)));

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <see cref="Expression"/>.Constant(<paramref name="index"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public BinaryExpression Index(long index)
		=> Expression.ArrayIndex(expression, Expression.Constant(index));

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <paramref name="indexes"/>.Select(index =&gt; (<see cref="Expression"/>)<see cref="Expression"/>.Constant(index)));</c>
	/// </remarks>
	public MethodCallExpression Index(long[] indexes)
		=> Expression.ArrayIndex(expression, indexes.Select(index => (Expression)Expression.Constant(index)));

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <paramref name="index"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public BinaryExpression Index(Expression index)
		=> Expression.ArrayIndex(expression, index);

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, IEnumerable{Expression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <paramref name="indexes"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public MethodCallExpression Index(IEnumerable<Expression> indexes)
		=> Expression.ArrayIndex(expression, indexes);

	/// <inheritdoc cref="Expression.ArrayIndex(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ArrayIndex(@<paramref name="this"/>, <paramref name="indexes"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public MethodCallExpression Index(Expression[] indexes)
		=> Expression.ArrayIndex(expression, indexes);
}
