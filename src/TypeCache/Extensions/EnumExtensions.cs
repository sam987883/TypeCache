// Copyright (c) 2021 Samuel Abraham

using System.Collections.Frozen;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Extensions;

public static partial class EnumExtensions
{
	extension<T>(T @this) where T : struct, Enum
	{
		[DebuggerHidden]
		public IReadOnlySet<Attribute> Attributes => typeof(T).Literals.TryGetValue(@this.Name, out var literal) ? literal.Attributes : FrozenSet<Attribute>.Empty;

		[DebuggerHidden]
		public bool HasAnyFlag(T[] flags)
			=> flags.Any(flag => @this.HasFlag(flag));

		/// <remarks>
		/// <c>=&gt; @this.ToString("X");</c>
		/// </remarks>
		[DebuggerHidden]
		public string Hex => @this.ToString("X");

		/// <remarks>
		/// <c>=&gt; <see cref="Enum{T}"/>.IsDefined(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool IsDefined()
			=> Enum<T>.IsDefined(@this);

		/// <remarks>
		/// <c>=&gt; @this.ToString("F");</c>
		/// </remarks>
		[DebuggerHidden]
		public string Name => @this.ToString("F");

		/// <remarks>
		/// <c>=&gt; @this.ToString("D");</c>
		/// </remarks>
		[DebuggerHidden]
		public string Number => @this.ToString("D");
	}

	extension(Enum @this)
	{
		/// <remarks>
		/// <c>=&gt; @this.ToString("X");</c>
		/// </remarks>
		[DebuggerHidden]
		public string Hex => @this.ToString("X");

		/// <remarks>
		/// <c>=&gt; @this.ToString("F");</c>
		/// </remarks>
		[DebuggerHidden]
		public string Name => @this.ToString("F");

		/// <remarks>
		/// <c>=&gt; @this.ToString("D");</c>
		/// </remarks>
		[DebuggerHidden]
		public string Number => @this.ToString("D");
	}

	extension(StringComparison @this)
	{
		/// <inheritdoc cref="StringComparer.FromComparison(StringComparison)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="StringComparer"/>.FromComparison(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public StringComparer ToComparer()
			=> StringComparer.FromComparison(@this);
	}
}
