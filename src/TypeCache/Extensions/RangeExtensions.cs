// Copyright (c) 2021 Samuel Abraham

using TypeCache.Utilities;

namespace TypeCache.Extensions;

public static class RangeExtensions
{
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static bool Any(this Range @this)
	{
		(@this.Start.IsFromEnd == @this.End.IsFromEnd).AssertTrue();

		return !@this.Start.Equals(@this.End);
	}

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach(this Range @this, Action<int> action)
	{
		action.AssertNotNull();

		foreach (var i in @this)
			action(i);
	}

	/// <summary>
	/// <c>=&gt; <see langword="new"/> <see cref="RangeEnumerator"/>(@<paramref name="this"/>);</c>
	/// </summary>
	public static RangeEnumerator GetEnumerator(this Range @this)
		=> new RangeEnumerator(@this);

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
			(true, true) => (@this.Start, other.Start).Min()..(@this.End, other.End).Max(),
			(true, false) => (@this.Start, other.End.Previous()).Min()..(@this.End, other.Start.Next()).Max(),
			(false, true) => (@this.Start, other.End.Next()).Max()..(@this.End, other.Start.Previous()).Min(),
			(false, false) => (@this.Start, other.Start).Max()..(@this.End, other.End).Min()
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
		=> (@this.End.Value - @this.Start.Value).Abs();

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

	public static IEnumerable<int> ToEnumerable(this Range @this)
	{
		foreach (var i in @this)
			yield return i;
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static Range? UnionWith(this Range @this, Range other)
		=> @this.Has(other.Start) || @this.Has(other.End) || other.Has(@this.Start) || other.Has(@this.End) ? (@this.IsReverse(), other.IsReverse()) switch
		{
			(true, true) => (@this.Start, other.Start).Max()..(@this.End, other.End).Min(),
			(true, false) => (@this.Start, other.End.Previous()).Max()..(@this.End, other.Start.Next()).Min(),
			(false, true) => (@this.Start, other.End.Next()).Min()..(@this.End, other.Start.Previous()).Max(),
			(false, false) => (@this.Start, other.Start).Min()..(@this.End, other.End).Max()
		} : null;
}
