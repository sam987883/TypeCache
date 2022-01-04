// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Math;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class RangeExtensions
{
	/// <summary>
	/// <c>=&gt; !@<paramref name="this"/>.Start.Equals(@<paramref name="this"/>.End);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool Any(this Range @this)
		=> !@this.Start.Equals(@this.End);

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/>.IsReverse() <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    true"/> =&gt; <paramref name="index"/>.Value &lt;= @<paramref name="this"/>.Start.Value &amp;&amp; <paramref name="index"/>.Value &gt; @<paramref name="this"/>.End.Value,<br/>
	///	<see langword="    false"/> =&gt; <paramref name="index"/>.Value &gt;= @<paramref name="this"/>.Start.Value &amp;&amp; <paramref name="index"/>.Value &lt; @<paramref name="this"/>.End.Value,<br/>
	///	<see langword="    "/>_ =&gt; <see langword="false"/><br/>
	/// };
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
	/// =&gt; (@<paramref name="this"/>.IsReverse(), <paramref name="other"/>.IsReverse()) <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    "/>(<see langword="true"/>, <see langword="true"/>) =&gt; <paramref name="other"/>.Start.Value &lt;= @<paramref name="this"/>.Start.Value &amp;&amp; <paramref name="other"/>.End.Value &gt;= @<paramref name="this"/>.End.Value,<br/>
	///	<see langword="    "/>(<see langword="true"/>, <see langword="false"/>) =&gt; <paramref name="other"/>.Start.Value &lt;= @<paramref name="this"/>.Start.Value &amp;&amp; <paramref name="other"/>.End.Value &gt; @<paramref name="this"/>.End.Value,<br/>
	///	<see langword="    "/>(<see langword="false"/>, <see langword="true"/>) =&gt; <paramref name="other"/>.Start.Value &gt; @<paramref name="this"/>.Start.Value &amp;&amp; <paramref name="other"/>.End.Value &lt;= @<paramref name="this"/>.End.Value,<br/>
	///	<see langword="    "/>(<see langword="false"/>, <see langword="false"/>) =&gt; <paramref name="other"/>.Start.Value &gt;= @<paramref name="this"/>.Start.Value &amp;&amp; <paramref name="other"/>.End.Value &lt;= @<paramref name="this"/>.End.Value,<br/>
	///	<see langword="    "/>_ =&gt; <see langword="false"/><br/>
	/// };
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
	/// <c>=&gt; @<paramref name="this"/>.Start.IsFromEnd == @<paramref name="this"/>.End.IsFromEnd ? @<paramref name="this"/>.Start.IsFromEnd : <see langword="null"/>;</c>
	/// </summary>
	public static bool? IsFromEnd(this Range @this)
		=> @this.Start.IsFromEnd == @this.End.IsFromEnd ? @this.Start.IsFromEnd : null;

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/>.Overlaps(<paramref name="other"/>) ? (@<paramref name="this"/>.IsReverse(), <paramref name="other"/>.IsReverse()) <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    "/>(<see langword="true"/>, <see langword="true"/>) =&gt; (@<paramref name="this"/>.Start, <paramref name="other"/>.Start).Minimum()..(@<paramref name="this"/>.End, <paramref name="other"/>.End).Maximum(),<br/>
	///	<see langword="    "/>(<see langword="true"/>, <see langword="false"/>) =&gt; (@<paramref name="this"/>.Start, <paramref name="other"/>.End.Previous()).Minimum()..(@<paramref name="this"/>.End, <paramref name="other"/>.Start.Next()).Maximum(),<br/>
	///	<see langword="    "/>(<see langword="false"/>, <see langword="true"/>) =&gt; (@<paramref name="this"/>.Start, <paramref name="other"/>.End.Next()).Maximum()..(@<paramref name="this"/>.End, <paramref name="other"/>.Start.Previous()).Minimum(),<br/>
	///	<see langword="    "/>(<see langword="false"/>, <see langword="false"/>) =&gt; (@<paramref name="this"/>.Start, <paramref name="other"/>.Start).Maximum()..(@<paramref name="this"/>.End, <paramref name="other"/>.End).Minimum(),<br/>
	///	<see langword="    "/>_ =&gt; <see langword="null"/><br/>
	/// } : <see langword="null"/>;
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
	/// =&gt; @<paramref name="this"/>.IsFromEnd() <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    "/>_ <see langword="when"/> @<paramref name="this"/>.Start.Equals(@<paramref name="this"/>.End) =&gt; <see langword="null"/>,<br/>
	///	<see langword="    true"/> =&gt; @<paramref name="this"/>.Start.Value &lt; @<paramref name="this"/>.End.Value,<br/>
	///	<see langword="    false"/> =&gt; @<paramref name="this"/>.Start.Value &gt; @<paramref name="this"/>.End.Value,<br/>
	///	<see langword="    "/>_ =&gt; <see langword="null"/><br/>
	/// };
	/// </code>
	/// </summary>
	/// <remarks>Reversal can only be determined if both Range Indexes have the same <c>IsFromEnd</c> value.</remarks>
	public static bool? IsReverse(this Range @this)
		=> @this.IsFromEnd() switch
		{
			_ when @this.Start.Equals(@this.End) => null,
			true => @this.Start.Value < @this.End.Value,
			false => @this.Start.Value > @this.End.Value,
			_ => null
		};

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>.End.Value - @<paramref name="this"/>.Start.Value);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static int Length(this Range @this)
		=> Abs(@this.End.Value - @this.Start.Value);

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/>.IsReverse() <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    true"/> =&gt; @<paramref name="this"/>.Start,<br/>
	///	<see langword="    false"/> =&gt; @<paramref name="this"/>.End.Previous(),<br/>
	///	<see langword="    "/>_ =&gt; <see langword="null"/><br/>
	///	};
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
	/// =&gt; @<paramref name="this"/>.IsReverse() <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    true"/> =&gt; @<paramref name="this"/>.End.Next(),<br/>
	///	<see langword="    false"/> =&gt; @<paramref name="this"/>.Start,<br/>
	///	<see langword="    "/>_ =&gt; <see langword="null"/><br/>
	///	};
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
	/// <c>=&gt; @<paramref name="this"/>.Start.Normalize(<paramref name="count"/>)..@<paramref name="this"/>.End.Normalize(<paramref name="count"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Range Normalize(this Range @this, int count)
		=> @this.Start.FromStart(count)..@this.End.FromStart(count);

	/// <summary>
	/// <code>
	/// =&gt; <paramref name="other"/>.IsReverse() <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    true"/> <see langword="when"/> @<paramref name="this"/>.Has(<paramref name="other"/>.Start) || @<paramref name="this"/>.Has(<paramref name="other"/>.End.Next()) =&gt; <see langword="true"/><br/>
	///	<see langword="    false"/> <see langword="when"/> @<paramref name="this"/>.Has(<paramref name="other"/>.Start) || @<paramref name="this"/>.Has(<paramref name="other"/>.End.Previous()) =&gt; <see langword="true"/><br/>
	///	<see langword="    "/>_ =&gt; <see langword="false"/><br/>
	/// };
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
	/// =&gt; @<paramref name="this"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    false"/> =&gt; @<paramref name="this"/>.End.Previous()..@<paramref name="this"/>.Start.Previous(),<br/>
	/// <see langword="    true"/> =&gt; @<paramref name="this"/>.End.Next()..@<paramref name="this"/>.Start.Next(),<br/>
	/// <see langword="    "/>_ =&gt; @<paramref name="this"/>.End..@<paramref name="this"/>.Start<br/>
	/// };
	/// </code>
	/// </summary>
	public static Range Reverse(this Range @this)
		=> @this.IsReverse() switch
		{
			false => @this.End.Previous()..@this.Start.Previous(),
			true => @this.End.Next()..@this.Start.Next(),
			_ => @this.End..@this.Start
		};

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/>.Has(<paramref name="other"/>.Start) || @<paramref name="this"/>.Has(<paramref name="other"/>.End) || <paramref name="other"/>.Has(@<paramref name="this"/>.Start) || <paramref name="other"/>.Has(@<paramref name="this"/>.End) ? (@<paramref name="this"/>.IsReverse(), <paramref name="other"/>.IsReverse()) <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    "/>(<see langword="true"/>, <see langword="true"/>) =&gt; (@<paramref name="this"/>.Start, <paramref name="other"/>.Start).Maximum()..(@<paramref name="this"/>.End, <paramref name="other"/>.End).Minimum(),<br/>
	///	<see langword="    "/>(<see langword="true"/>, <see langword="false"/>) =&gt; (@<paramref name="this"/>.Start, <paramref name="other"/>.End.Previous()).Maximum()..(@<paramref name="this"/>.End, <paramref name="other"/>.Start.Next()).Minimum(),<br/>
	///	<see langword="    "/>(<see langword="false"/>, <see langword="true"/>) =&gt; (@<paramref name="this"/>.Start, <paramref name="other"/>.End.Next()).Minimum()..(@<paramref name="this"/>.End, <paramref name="other"/>.Start.Previous()).Maximum(),<br/>
	///	<see langword="    "/>(<see langword="false"/>, <see langword="false"/>) =&gt; (@<paramref name="this"/>.Start, <paramref name="other"/>.Start).Minimum()..(@<paramref name="this"/>.End, <paramref name="other"/>.End).Maximum(),<br/>
	///	<see langword="    "/>_ =&gt; <see langword="null"/><br/>
	/// } : <see langword="null"/>;
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
	/// <see langword="var"/> reverse = @<paramref name="this"/>.IsReverse();<br/>
	/// <see langword="if"/> (reverse <see langword="is false"/>)<br/>
	///	<see langword="    for"/> (<see langword="var"/> i = @<paramref name="this"/>.Start.Value; i &lt; @<paramref name="this"/>.End.Value; ++i)<br/>
	///	<see langword="        yield return"/> i;<br/>
	/// <see langword="if"/> (reverse <see langword="is true"/>)<br/>
	///	<see langword="    for"/> (<see langword="var"/> i = @<paramref name="this"/>.Start.Value; i &gt; @<paramref name="this"/>.End.Value; --i)<br/>
	///	<see langword="        yield return"/> i;<br/>
	/// <see langword="yield break"/>;
	/// </code>
	/// </summary>
	public static IEnumerable<int> Values(this Range @this)
	{
		var reverse = @this.IsReverse();
		if (reverse is false)
			for (var i = @this.Start.Value; i < @this.End.Value; ++i)
				yield return i;
		else if (reverse is true)
			for (var i = @this.Start.Value; i > @this.End.Value; --i)
				yield return i;
		yield break;
	}
}
