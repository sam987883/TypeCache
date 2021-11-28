// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using static System.Math;
using static TypeCache.Default;

namespace TypeCache.Extensions
{
	public static class RangeExtensions
	{
		/// <summary>
		/// <c>!@<paramref name="this"/>.Start.Equals(@<paramref name="this"/>.End)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static bool Any(this Range @this)
			=> !@this.Start.Equals(@this.End);

		/// <summary>
		/// <c><see cref="Math"/>.Abs(@<paramref name="this"/>.End.Value - @<paramref name="this"/>.Start.Value)</c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static int Count(this Range @this)
			=> Abs(@this.End.Value - @this.Start.Value);

		/// <summary>
		/// <code>
		/// @<paramref name="this"/>.IsReverse() <see langword="switch"/><br/>
		/// {<br/>
		///		<see langword="true"/> =&gt; <paramref name="index"/>.Value &lt;= @<paramref name="this"/>.Start.Value &amp;&amp; <paramref name="index"/>.Value &gt; @<paramref name="this"/>.End.Value,<br/>
		///		<see langword="false"/> =&gt; <paramref name="index"/>.Value &gt;= @<paramref name="this"/>.Start.Value &amp;&amp; <paramref name="index"/>.Value &lt; @<paramref name="this"/>.End.Value,<br/>
		///		_ =&gt; <see langword="false"/><br/>
		/// }<br/>
		/// </code>
		/// </summary>
		public static bool Has(this Range @this, Index index)
			=> @this.IsReverse() switch
			{
				true => index.Value <= @this.Start.Value && index.Value > @this.End.Value,
				false => index.Value >= @this.Start.Value && index.Value < @this.End.Value,
				_ => false
			};

		/// <summary>
		/// <code>
		/// (@<paramref name="this"/>.IsReverse(), <paramref name="other"/>.IsReverse()) <see langword="switch"/><br/>
		/// {<br/>
		///		(<see langword="true"/>, <see langword="true"/>) =&gt; <paramref name="other"/>.Start.Value &lt;= @<paramref name="this"/>.Start.Value &amp;&amp; <paramref name="other"/>.End.Value &gt;= @<paramref name="this"/>.End.Value,<br/>
		///		(<see langword="true"/>, <see langword="false"/>) =&gt; <paramref name="other"/>.Start.Value &lt;= @<paramref name="this"/>.Start.Value &amp;&amp; <paramref name="other"/>.End.Value &gt; @<paramref name="this"/>.End.Value,<br/>
		///		(<see langword="false"/>, <see langword="true"/>) =&gt; <paramref name="other"/>.Start.Value &gt; @<paramref name="this"/>.Start.Value &amp;&amp; <paramref name="other"/>.End.Value &lt;= @<paramref name="this"/>.End.Value,<br/>
		///		(<see langword="false"/>, <see langword="false"/>) =&gt; <paramref name="other"/>.Start.Value &gt;= @<paramref name="this"/>.Start.Value &amp;&amp; <paramref name="other"/>.End.Value &lt;= @<paramref name="this"/>.End.Value,<br/>
		///		_ =&gt; <see langword="false"/><br/>
		/// }<br/>
		/// </code>
		/// </summary>
		public static bool Has(this Range @this, Range other)
			=> (@this.IsReverse(), other.IsReverse()) switch
			{
				(true, true) => other.Start.Value <= @this.Start.Value && other.End.Value >= @this.End.Value,
				(true, false) => other.End.Value <= @this.Start.Value && other.Start.Value > @this.End.Value,
				(false, true) => other.Start.Value > @this.End.Value && other.End.Value <= @this.Start.Value,
				(false, false) => other.Start.Value >= @this.Start.Value && other.End.Value <= @this.End.Value,
				_ => false
			};

		/// <summary>
		/// <code>
		/// @<paramref name="this"/>.Overlaps(<paramref name="other"/>) ? (@<paramref name="this"/>.IsReverse(), <paramref name="other"/>.IsReverse()) <see langword="switch"/><br/>
		/// {<br/>
		///		(<see langword="true"/>, <see langword="true"/>) =&gt; (@<paramref name="this"/>.Start, <paramref name="other"/>.Start).Minimum()..(@<paramref name="this"/>.End, <paramref name="other"/>.End).Maximum(),<br/>
		///		(<see langword="true"/>, <see langword="false"/>) =&gt; (@<paramref name="this"/>.Start, <paramref name="other"/>.End.Previous()).Minimum()..(@<paramref name="this"/>.End, <paramref name="other"/>.Start.Next()).Maximum(),<br/>
		///		(<see langword="false"/>, <see langword="true"/>) =&gt; (@<paramref name="this"/>.Start, <paramref name="other"/>.End.Next()).Maximum()..(@<paramref name="this"/>.End, <paramref name="other"/>.Start.Previous()).Minimum(),<br/>
		///		(<see langword="false"/>, <see langword="false"/>) =&gt; (@<paramref name="this"/>.Start, <paramref name="other"/>.Start).Maximum()..(@<paramref name="this"/>.End, <paramref name="other"/>.End).Minimum(),<br/>
		///		_ =&gt; <see langword="null"/><br/>
		/// } : <see langword="null"/><br/>
		/// </code>
		/// </summary>
		public static Range? IntersectWith(this Range @this, Range other)
			=> @this.Overlaps(other) ? (@this.IsReverse(), other.IsReverse()) switch
			{
				(true, true) => (@this.Start, other.Start).Minimum()..(@this.End, other.End).Maximum(),
				(true, false) => (@this.Start, other.End.Previous()).Minimum()..(@this.End, other.Start.Next()).Maximum(),
				(false, true) => (@this.Start, other.End.Next()).Maximum()..(@this.End, other.Start.Previous()).Minimum(),
				(false, false) => (@this.Start, other.Start).Maximum()..(@this.End, other.End).Minimum(),
				_ => null
			} : null;

		/// <summary>
		/// <code>
		/// (@<paramref name="this"/>.Start.IsFromEnd, @<paramref name="this"/>.End.IsFromEnd) <see langword="switch"/><br/>
		/// {<br/>
		///		_ <see langword="when"/> @<paramref name="this"/>.Start.Equals(@<paramref name="this"/>.End) =&gt; <see langword="null"/>,<br/>
		///		(<see langword="true"/>, <see langword="true"/>) =&gt; @<paramref name="this"/>.Start.Value &lt; @<paramref name="this"/>.End.Value,<br/>
		///		(<see langword="false"/>, <see langword="false"/>) =&gt; @<paramref name="this"/>.Start.Value &gt; @<paramref name="this"/>.End.Value,<br/>
		///		_ =&gt; <see langword="null"/><br/>
		/// }<br/>
		/// </code>
		/// </summary>
		/// <remarks>Reversal can only be determined if both Range Indexes have the same <c>IsFromEnd</c> value.</remarks>
		public static bool? IsReverse(this Range @this)
			=> (@this.Start.IsFromEnd, @this.End.IsFromEnd) switch
			{
				_ when @this.Start.Equals(@this.End) => null,
				(true, true) => @this.Start.Value < @this.End.Value,
				(false, false) => @this.Start.Value > @this.End.Value,
				_ => null
			};

		/// <summary>
		/// <code>
		/// @<paramref name="this"/>.IsReverse() <see langword="switch"/><br/>
		/// {<br/>
		///		<see langword="true"/> =&gt; @<paramref name="this"/>.Start,<br/>
		///		<see langword="false"/> =&gt; @<paramref name="this"/>.End.Previous(),<br/>
		///		_ =&gt; <see langword="null"/><br/>
		///	}<br/>
		///	</code>
		/// </summary>
		public static Index? Maximum(this Range @this)
			=> @this.IsReverse() switch
			{
				true => @this.Start,
				false => @this.End.Previous(),
				_ => null
			};

		/// <summary>
		/// <code>
		/// @<paramref name="this"/>.IsReverse() <see langword="switch"/><br/>
		/// {<br/>
		///		<see langword="true"/> =&gt; @<paramref name="this"/>.End.Next(),<br/>
		///		<see langword="false"/> =&gt; @<paramref name="this"/>.Start,<br/>
		///		_ =&gt; <see langword="null"/><br/>
		///	}<br/>
		///	</code>
		/// </summary>
		public static Index? Minimum(this Range @this)
			=> @this.IsReverse() switch
			{
				true => @this.End.Next(),
				false => @this.Start,
				_ => null
			};

		/// <summary>
		/// <c>@<paramref name="this"/>.Start.Normalize(<paramref name="count"/>)..@<paramref name="this"/>.End.Normalize(<paramref name="count"/>)</c>
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"/>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static Range Normalize(this Range @this, int count)
			=> @this.Start.Normalize(count)..@this.End.Normalize(count);

		/// <summary>
		/// <code>
		/// <paramref name="other"/>.IsReverse() <see langword="switch"/><br/>
		/// {<br/>
		///		<see langword="true"/> <see langword="when"/> @<paramref name="this"/>.Has(<paramref name="other"/>.Start) || @<paramref name="this"/>.Has(<paramref name="other"/>.End.Next()) =&gt; <see langword="true"/><br/>
		///		<see langword="false"/> <see langword="when"/> @<paramref name="this"/>.Has(<paramref name="other"/>.Start) || @<paramref name="this"/>.Has(<paramref name="other"/>.End.Previous()) =&gt; <see langword="true"/><br/>
		///		_ =&gt; <see langword="false"/><br/>
		/// }<br/>
		/// </code>
		/// </summary>
		public static bool Overlaps(this Range @this, Range other)
			=> other.IsReverse() switch
			{
				true when @this.Has(other.Start) || @this.Has(other.End.Next()) => true,
				false when @this.Has(other.Start) || @this.Has(other.End.Previous()) => true,
				_ => false
			};

		/// <summary>
		/// <code>
		/// @<paramref name="this"/>.Has(<paramref name="other"/>.Start)
		/// || @<paramref name="this"/>.Has(<paramref name="other"/>.End)
		/// || <paramref name="other"/>.Has(@<paramref name="this"/>.Start)
		/// || <paramref name="other"/>.Has(@<paramref name="this"/>.End)
		/// ? (@<paramref name="this"/>.IsReverse(), <paramref name="other"/>.IsReverse()) <see langword="switch"/><br/>
		/// {<br/>
		///		(<see langword="true"/>, <see langword="true"/>) =&gt; (@<paramref name="this"/>.Start, <paramref name="other"/>.Start).Maximum()..(@<paramref name="this"/>.End, <paramref name="other"/>.End).Minimum(),<br/>
		///		(<see langword="true"/>, <see langword="false"/>) =&gt; (@<paramref name="this"/>.Start, <paramref name="other"/>.End.Previous()).Maximum()..(@<paramref name="this"/>.End, <paramref name="other"/>.Start.Next()).Minimum(),<br/>
		///		(<see langword="false"/>, <see langword="true"/>) =&gt; (@<paramref name="this"/>.Start, <paramref name="other"/>.End.Next()).Minimum()..(@<paramref name="this"/>.End, <paramref name="other"/>.Start.Previous()).Maximum(),<br/>
		///		(<see langword="false"/>, <see langword="false"/>) =&gt; (@<paramref name="this"/>.Start, <paramref name="other"/>.Start).Minimum()..(@<paramref name="this"/>.End, <paramref name="other"/>.End).Maximum(),<br/>
		///		_ =&gt; <see langword="null"/><br/>
		/// } : <see langword="null"/><br/>
		/// </code>
		/// </summary>
		public static Range? UnionWith(this Range @this, Range other)
			=> @this.Has(other.Start) || @this.Has(other.End) || other.Has(@this.Start) || other.Has(@this.End) ? (@this.IsReverse(), other.IsReverse()) switch
			{
				(true, true) => (@this.Start, other.Start).Maximum()..(@this.End, other.End).Minimum(),
				(true, false) => (@this.Start, other.End.Previous()).Maximum()..(@this.End, other.Start.Next()).Minimum(),
				(false, true) => (@this.Start, other.End.Next()).Minimum()..(@this.End, other.Start.Previous()).Maximum(),
				(false, false) => (@this.Start, other.Start).Minimum()..(@this.End, other.End).Maximum(),
				_ => null
			} : null;

		/// <summary>
		/// <code>
		/// @<paramref name="this"/>.IsReverse() <see langword="switch"/><br/>
		/// {<br/>
		///		<see langword="true"/> =&gt; @<paramref name="this"/>.Start.Value.Range(@<paramref name="this"/>.Start.Value - @<paramref name="this"/>.End.Value, -1),<br/>
		///		<see langword="false"/> =&gt; @<paramref name="this"/>.Start.Value.Range(@<paramref name="this"/>.End.Value - @<paramref name="this"/>.Start.Value, 1),<br/>
		///		_ =&gt; Enumerable&lt;<see cref="int"/>&gt;.Empty<br/>
		/// }<br/>
		/// </code>
		/// </summary>
		public static IEnumerable<int> Values(this Range @this)
			=> @this.IsReverse() switch
			{
				true => @this.Start.Value.Range(@this.Start.Value - @this.End.Value, -1),
				false => @this.Start.Value.Range(@this.End.Value - @this.Start.Value, 1),
				_ => Enumerable<int>.Empty
			};
	}
}
