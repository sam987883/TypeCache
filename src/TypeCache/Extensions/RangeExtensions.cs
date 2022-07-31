// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static System.Math;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class RangeExtensions
{
	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Start.IsFromEnd == @<paramref name="this"/>.End.IsFromEnd
	/// ? !@<paramref name="this"/>.Start.Equals(@<paramref name="this"/>.End)
	/// : <see langword="null"/>;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool? Any(this Range @this)
		=> @this.Start.IsFromEnd == @this.End.IsFromEnd ? !@this.Start.Equals(@this.End) : null;

	/// <summary>
	/// <code>
	/// <paramref name="action"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="var"/> reverse = @<paramref name="this"/>.IsReverse();<br/>
	/// <see langword="if"/> (!reverse.HasValue)<br/>
	/// <see langword="    return"/>;<br/>
	/// <br/>
	/// <see langword="var"/> end = @<paramref name="this"/>.End.Value;<br/>
	/// <see langword="var"/> increment = reverse.Value ? -1 : 1;<br/>
	/// <see langword="for"/> (<see langword="var"/> i = @<paramref name="this"/>.Start.Value; i != end; i += increment)<br/>
	/// <see langword="    "/> <paramref name="action"/>(i);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static void Do(this Range @this, Action<int> action)
	{
		action.AssertNotNull();

		var reverse = @this.IsReverse();
		if (!reverse.HasValue)
			return;

		var end = @this.End.Value;
		var increment = reverse.Value ? -1 : 1;
		for (var i = @this.Start.Value; i != end; i += increment)
			action(i);
	}

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
	///	<see langword="    "/>_ <see langword="when"/> @this.Any() <see langword="is not true"/> &amp;&amp; other.Any() <see langword="is not true"/> =&gt; <see langword="false"/>,<br/>
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
			_ when @this.Any() is not true && other.Any() is not true => false,
			(true, true) => other.Start.Value <= @this.Start.Value && other.End.Value >= @this.End.Value,
			(true, false) => other.End.Value <= @this.Start.Value && other.Start.Value > @this.End.Value,
			(false, true) => other.Start.Value > @this.End.Value && other.End.Value <= @this.Start.Value,
			(false, false) => other.Start.Value >= @this.Start.Value && other.End.Value <= @this.End.Value,
			_ => false
		};

	/// <summary>
	/// <code>
	/// <paramref name="condition"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="var"/> reverse = @<paramref name="this"/>.IsReverse();<br/>
	/// <see langword="if"/> (!reverse.HasValue)<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="var"/> end = @<paramref name="this"/>.End.Value;<br/>
	/// <see langword="var"/> increment = reverse.Value ? -1 : 1;<br/>
	/// <see langword="for"/> (<see langword="var"/> i = @<paramref name="this"/>.Start.Value; i != end; i += increment)<br/>
	/// <see langword="    if"/> (<paramref name="condition"/>(i))<br/>
	/// <see langword="        yield return"/> i;
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<int> If(this Range @this, Predicate<int> condition)
	{
		condition.AssertNotNull();

		var reverse = @this.IsReverse();
		if (!reverse.HasValue)
			yield break;

		var end = @this.End.Value;
		var increment = reverse.Value ? -1 : 1;
		for (var i = @this.Start.Value; i != end; i += increment)
			if (condition(i))
				yield return i;
	}

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
	/// =&gt; (@<paramref name="this"/>.Start.IsFromEnd, @<paramref name="this"/>.End.IsFromEnd) <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    "/>(<see langword="true"/>, <see langword="true"/>) =&gt; @<paramref name="this"/>.Start.Value &lt; @<paramref name="this"/>.End.Value,<br/>
	///	<see langword="    "/>(<see langword="false"/>, <see langword="false"/>) =&gt; @<paramref name="this"/>.Start.Value &gt; @<paramref name="this"/>.End.Value,<br/>
	///	<see langword="    "/>_ =&gt; <see langword="null"/><br/>
	/// };
	/// </code>
	/// </summary>
	/// <remarks>Reversal can only be determined if both Range Indexes have the same <c>IsFromEnd</c> value.</remarks>
	public static bool? IsReverse(this Range @this)
		=> (@this.Start.IsFromEnd, @this.End.IsFromEnd) switch
		{
			(true, true) => @this.Start.Value < @this.End.Value,
			(false, false) => @this.Start.Value > @this.End.Value,
			_ => null
		};

	/// <summary>
	/// <c>=&gt; <see cref="Math"/>.Abs(@<paramref name="this"/>.End.Value - @<paramref name="this"/>.Start.Value);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static int Length(this Range @this)
		=> Abs(@this.End.Value - @this.Start.Value);

	/// <summary>
	/// <code>
	/// <paramref name="map"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="var"/> reverse = @<paramref name="this"/>.IsReverse();<br/>
	/// <see langword="if"/> (!reverse.HasValue)<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="var"/> end = @<paramref name="this"/>.End.Value;<br/>
	/// <see langword="var"/> increment = reverse.Value ? -1 : 1;<br/>
	/// <see langword="for"/> (<see langword="var"/> i = @<paramref name="this"/>.Start.Value; i != end; i += increment)<br/>
	/// <see langword="    yield return"/> <paramref name="map"/>(i);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<T> Map<T>(this Range @this, Func<int, T> map)
	{
		map.AssertNotNull();

		var reverse = @this.IsReverse();
		if (!reverse.HasValue)
			yield break;

		var start = @this.Start.Value;
		var end = @this.End.Value;
		if (reverse.Value)
			for (var i = start - 1; i >= end; --i)
				yield return map(i);
		else
			for (var i = start; i < end; ++i)
				yield return map(i);
	}

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/>.IsReverse() <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    true when"/> @<paramref name="this"/>.Start.Value &gt; @<paramref name="this"/>.End.Value =&gt; @<paramref name="this"/>.Start,<br/>
	///	<see langword="    false when"/> @<paramref name="this"/>.Start.Value &lt; @<paramref name="this"/>.End.Value =&gt; @<paramref name="this"/>.End.Previous(),<br/>
	///	<see langword="    "/>_ =&gt; <see langword="null"/><br/>
	///	};
	///	</code>
	/// </summary>
	public static Index? Maximum(this Range @this)
		=> @this.IsReverse() switch
		{
			true when @this.Start.Value > @this.End.Value => @this.Start,
			false when @this.Start.Value < @this.End.Value => @this.End.Previous(),
			_ => null
		};

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/>.IsReverse() <see langword="switch"/><br/>
	/// {<br/>
	///	<see langword="    true when"/> @<paramref name="this"/>.Start.Value &gt; @<paramref name="this"/>.End.Value =&gt; @<paramref name="this"/>.End.Next(),<br/>
	///	<see langword="    false when"/> @<paramref name="this"/>.Start.Value &lt; @<paramref name="this"/>.End.Value =&gt; @<paramref name="this"/>.Start,<br/>
	///	<see langword="    "/>_ =&gt; <see langword="null"/><br/>
	///	};
	///	</code>
	/// </summary>
	public static Index? Minimum(this Range @this)
		=> @this.IsReverse() switch
		{
			true when @this.Start.Value > @this.End.Value => @this.End.Next(),
			false when @this.Start.Value < @this.End.Value => @this.Start,
			_ => null
		};

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Start.FromStart(<paramref name="count"/>)..@<paramref name="this"/>.End.FromStart(<paramref name="count"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
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
	/// <c>=&gt; <see langword="new"/> <see cref="Range"/>(@<paramref name="this"/>.End, @<paramref name="this"/>.Start);</c>
	/// </summary>
	public static Range Reverse(this Range @this)
		=> new Range(@this.End, @this.Start);

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
	/// <see langword="if"/> (!reverse.HasValue)<br/>
	/// <see langword="    yield break"/>;<br/>
	/// <br/>
	/// <see langword="var"/> increment = reverse.Value ? -1 : 1;<br/>
	/// <see langword="var"/> end = @<paramref name="this"/>.End.Value + increment;<br/>
	/// <see langword="for"/> (<see langword="var"/> i = @<paramref name="this"/>.Start.Value; i != end; i += increment)<br/>
	/// <see langword="    yield return"/> i;
	/// </code>
	/// </summary>
	public static IEnumerable<int> Values(this Range @this)
	{
		var reverse = @this.IsReverse();
		if (!reverse.HasValue)
			yield break;

		var increment = reverse.Value ? -1 : 1;
		var end = @this.End.Value + increment;
		for (var i = @this.Start.Value; i != end; i += increment)
			yield return i;
	}
}
