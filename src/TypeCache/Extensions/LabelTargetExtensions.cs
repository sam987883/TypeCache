// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;

namespace TypeCache.Extensions;

public static class LabelTargetExtensions
{
	/// <inheritdoc cref="Expression.Break(LabelTarget)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Break(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static GotoExpression Break(this LabelTarget @this)
		=> Expression.Break(@this);

	/// <inheritdoc cref="Expression.Break(LabelTarget, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Break(@<paramref name="this"/>, <paramref name="value"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static GotoExpression Break(this LabelTarget @this, Expression? value)
		=> Expression.Break(@this, value);

	/// <inheritdoc cref="Expression.Break(LabelTarget, Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Break(@<paramref name="this"/>, <paramref name="value"/>, <paramref name="type"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static GotoExpression Break(this LabelTarget @this, Expression? value, Type type)
		=> Expression.Break(@this, value, type);

	/// <inheritdoc cref="Expression.Break(LabelTarget, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Break(@<paramref name="this"/>, <paramref name="type"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static GotoExpression Break(this LabelTarget @this, Type type)
		=> Expression.Break(@this, type);

	/// <inheritdoc cref="Expression.Continue(LabelTarget)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Continue(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static GotoExpression Continue(this LabelTarget @this)
		=> Expression.Continue(@this);

	/// <inheritdoc cref="Expression.Continue(LabelTarget, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Continue(@<paramref name="this"/>, <paramref name="type"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static GotoExpression Continue(this LabelTarget @this, Type type)
		=> Expression.Continue(@this, type);

	/// <inheritdoc cref="Expression.Goto(LabelTarget)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Goto(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static GotoExpression Goto(this LabelTarget @this)
		=> Expression.Goto(@this);

	/// <inheritdoc cref="Expression.Label(LabelTarget)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Label(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static LabelExpression Label(this LabelTarget @this)
		=> Expression.Label(@this);

	/// <inheritdoc cref="Expression.Label(LabelTarget, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Label(@<paramref name="this"/>, <paramref name="defaultValue"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static LabelExpression Label(this LabelTarget @this, Expression? defaultValue)
		=> Expression.Label(@this, defaultValue);
}
