// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;
using TypeCache.Reflection;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public static class GlobalExtensions
{
	extension<T>(T @this)
	{
		public IEnumerable<T> Repeat(int count)
		{
			while (--count > -1)
				yield return @this;
		}

		/// <inheritdoc cref="Tuple{T1}"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Tuple"/>.Create(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Tuple<T> ToTuple()
			=> Tuple.Create(@this);

		/// <inheritdoc cref="ValueTuple{T1}"/>
		/// <remarks>
		/// <c>=&gt; <see cref="ValueTuple"/>.Create(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public ValueTuple<T> ToValueTuple()
			=> ValueTuple.Create(@this);
	}

	extension<T>(T? @this)
	{
		/// <inheritdoc cref="Expression.Constant(object?, Type)"/>
		public ConstantExpression ConstantExpression()
			=> @this is not null ? Expression.Constant(@this, @this.GetType()) : Expression.Constant(null);

		/// <summary>
		/// Returns the C# code of the expression.
		/// </summary>
		/// <remarks>
		/// <c>=&gt; <paramref name="code"/> ?? <see cref="string.Empty"/>;</c>
		/// </remarks>
		/// <param name="code">Leave this parameter as <c><see langword="null"/></c></param>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToCode([AllowNull][CallerArgumentExpression(nameof(@this))] string? code = null)
			=> code ?? string.Empty;
	}

	extension<T>(T @this) where T : class
	{
		/// <inheritdoc cref="WeakReference{T}"/>
		/// <remarks>
		/// <c>=&gt; <see langword="new"/>(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public WeakReference<T> ToWeakReference()
			=> new(@this);
	}

	extension<T>(T @this) where T : struct
	{
		/// <remarks>
		/// <c>=&gt; <see cref="ValueBox{T}"/>.GetValue(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public object Box()
			=> ValueBox<T>.GetValue(@this);

		/// <inheritdoc cref="StrongBox{T}.StrongBox(T)"/>
		/// <remarks>
		/// <c>=&gt; <see langword="new"/>(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public StrongBox<T> ToStrongBox()
			=> new(@this);
	}

	extension<T>(T @this) where T : IComparisonOperators<T, T, bool>
	{
		/// <remarks>
		/// <c>=&gt; @this &gt;= <paramref name="minimum"/> &amp;&amp; @this &lt;= <paramref name="maximum"/>;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool Between(T minimum, T maximum)
			=> @this >= minimum && @this <= maximum;

		/// <remarks>
		/// <c>=&gt; @this &gt; <paramref name="minimum"/> &amp;&amp; @this &lt; <paramref name="maximum"/>;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool InBetween(T minimum, T maximum)
			=> @this > minimum && @this < maximum;
	}
}
