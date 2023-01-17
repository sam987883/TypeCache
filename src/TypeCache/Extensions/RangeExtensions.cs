// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public static class RangeExtensions
{
	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Start.IsFromEnd == @<paramref name="this"/>.End.IsFromEnd
	/// ? !@<paramref name="this"/>.Start.Equals(@<paramref name="this"/>.End)
	/// : <see langword="null"/>;</c>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException"/>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Any(this Range @this)
	{
		(@this.Start.IsFromEnd == @this.End.IsFromEnd).AssertTrue();

		return !@this.Start.Equals(@this.End);
	}

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static void ForEach(this Range @this, Action<int> action)
	{
		action.AssertNotNull();

		var end = @this.End.Value;
		var increment = @this.IsReverse() ? -1 : 1;
		for (var i = @this.Start.Value; i != end; i += increment)
			action(i);
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static bool Has(this Range @this, Index index)
		=> @this.IsReverse()
			? index.Value <= @this.Start.Value && index.Value > @this.End.Value
			: index.Value >= @this.Start.Value && index.Value < @this.End.Value;

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static bool Has(this Range @this, Range other)
		=> (@this.IsReverse(), other.IsReverse()) switch
		{
			_ when !@this.Any() && !other.Any() => false,
			(true, true) => other.Start.Value <= @this.Start.Value && other.End.Value >= @this.End.Value,
			(true, false) => other.End.Value <= @this.Start.Value && other.Start.Value > @this.End.Value,
			(false, true) => other.Start.Value > @this.End.Value && other.End.Value <= @this.Start.Value,
			(false, false) => other.Start.Value >= @this.Start.Value && other.End.Value <= @this.End.Value
		};

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static Range? IntersectWith(this Range @this, Range other)
		=> @this.Overlaps(other) ? (@this.IsReverse(), other.IsReverse()) switch
		{
			(true, true) => (@this.Start, other.Start).Minimum()..(@this.End, other.End).Maximum(),
			(true, false) => (@this.Start, other.End.Previous()).Minimum()..(@this.End, other.Start.Next()).Maximum(),
			(false, true) => (@this.Start, other.End.Next()).Maximum()..(@this.End, other.Start.Previous()).Minimum(),
			(false, false) => (@this.Start, other.Start).Maximum()..(@this.End, other.End).Minimum()
		} : null;

	/// <remarks>Reversal can only be determined if both Range Indexes have the same <c>IsFromEnd</c> value.</remarks>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static bool IsReverse(this Range @this)
	{
		(@this.Start.IsFromEnd == @this.End.IsFromEnd).AssertTrue();

		return @this.Start.IsFromEnd ? @this.Start.Value <= @this.End.Value : @this.Start.Value > @this.End.Value;
	}

	/// <summary>
	/// <c>=&gt; (@<paramref name="this"/>.End.Value - @<paramref name="this"/>.Start.Value).AbsoluteValue();</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int Length(this Range @this)
		=> (@this.End.Value - @this.Start.Value).AbsoluteValue();

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static IEnumerable<T> Select<T>(this Range @this, Func<int, T> map)
	{
		map.AssertNotNull();

		var start = @this.Start.Value;
		var end = @this.End.Value;
		if (@this.IsReverse())
			for (var i = start - 1; i >= end; --i)
				yield return map(i);
		else
			for (var i = start; i < end; ++i)
				yield return map(i);
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static Index? Maximum(this Range @this)
		=> @this.IsReverse() switch
		{
			true when @this.Start.Value > @this.End.Value => @this.Start,
			false when @this.Start.Value < @this.End.Value => @this.End.Previous(),
			_ => null
		};

	/// <exception cref="ArgumentOutOfRangeException"/>
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
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Range Normalize(this Range @this, int count)
		=> @this.Start.FromStart(count)..@this.End.FromStart(count);

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static bool Overlaps(this Range @this, Range other)
		=> other.IsReverse()
			? @this.Has(other.Start) || @this.Has(other.End.Next())
			: @this.Has(other.Start) || @this.Has(other.End.Previous());

	/// <summary>
	/// <c>=&gt; <see langword="new"/> <see cref="Range"/>(@<paramref name="this"/>.End, @<paramref name="this"/>.Start);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Range Reverse(this Range @this)
		=> new Range(@this.End, @this.Start);

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static Range? UnionWith(this Range @this, Range other)
		=> @this.Has(other.Start) || @this.Has(other.End) || other.Has(@this.Start) || other.Has(@this.End) ? (@this.IsReverse(), other.IsReverse()) switch
		{
			(true, true) => (@this.Start, other.Start).Maximum()..(@this.End, other.End).Minimum(),
			(true, false) => (@this.Start, other.End.Previous()).Maximum()..(@this.End, other.Start.Next()).Minimum(),
			(false, true) => (@this.Start, other.End.Next()).Minimum()..(@this.End, other.Start.Previous()).Maximum(),
			(false, false) => (@this.Start, other.Start).Minimum()..(@this.End, other.End).Maximum()
		} : null;

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static IEnumerable<int> Values(this Range @this)
	{
		var increment = @this.IsReverse() ? -1 : 1;
		var end = @this.End.Value + increment;
		for (var i = @this.Start.Value; i != end; i += increment)
			yield return i;
	}

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static IEnumerable<int> Where(this Range @this, Predicate<int> condition)
	{
		condition.AssertNotNull();

		var end = @this.End.Value;
		var increment = @this.IsReverse() ? -1 : 1;
		for (var i = @this.Start.Value; i != end; i += increment)
			if (condition(i))
				yield return i;
	}
}
