// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;

namespace TypeCache.Extensions;

public static class LabelTargetExtensions
{
	extension(LabelTarget @this)
	{
		/// <inheritdoc cref="Expression.Break(LabelTarget)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Break(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public GotoExpression Break()
			=> Expression.Break(@this);

		/// <inheritdoc cref="Expression.Break(LabelTarget, Expression)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Break(@this, <paramref name="value"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public GotoExpression Break(Expression? value)
			=> Expression.Break(@this, value);

		/// <inheritdoc cref="Expression.Break(LabelTarget, Expression, Type)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Break(@this, <paramref name="value"/>, <paramref name="type"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public GotoExpression Break(Expression? value, Type type)
			=> Expression.Break(@this, value, type);

		/// <inheritdoc cref="Expression.Break(LabelTarget, Type)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Break(@this, <paramref name="type"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public GotoExpression Break(Type type)
			=> Expression.Break(@this, type);

		/// <inheritdoc cref="Expression.Continue(LabelTarget)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Continue(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public GotoExpression Continue()
			=> Expression.Continue(@this);

		/// <inheritdoc cref="Expression.Continue(LabelTarget, Type)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Continue(@this, <paramref name="type"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public GotoExpression Continue(Type type)
			=> Expression.Continue(@this, type);

		/// <inheritdoc cref="Expression.Goto(LabelTarget)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Goto(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public GotoExpression Goto()
			=> Expression.Goto(@this);

		/// <inheritdoc cref="Expression.Label(LabelTarget)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Label(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public LabelExpression Label()
			=> Expression.Label(@this);

		/// <inheritdoc cref="Expression.Label(LabelTarget, Expression)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Label(@this, <paramref name="defaultValue"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public LabelExpression Label(Expression? defaultValue)
			=> Expression.Label(@this, defaultValue);
	}

	extension(Type @this)
	{
		/// <inheritdoc cref="Expression.Label(Type)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Label(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public LabelTarget ToLabelTarget()
			=> Expression.Label(@this);

		/// <inheritdoc cref="Expression.Label(Type, string)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Label(@this, <paramref name="name"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public LabelTarget ToLabelTarget(string? name)
			=> Expression.Label(@this, name);
	}

	extension(string? @this)
	{
		/// <inheritdoc cref="Expression.Label(string)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Label(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public LabelTarget ToLabelTarget()
			=> Expression.Label(@this);
	}
}
