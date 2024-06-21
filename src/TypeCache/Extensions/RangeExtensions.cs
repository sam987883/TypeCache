// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public static class RangeExtensions
{
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static bool Any(this Range @this)
	{
		(@this.Start.IsFromEnd == @this.End.IsFromEnd).ThrowIfFalse();

		return !@this.Start.Equals(@this.End);
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static bool Contains(this Range @this, Index index)
		=> @this.IsReverse()
			? index.Value < @this.Start.Value && index.Value >= @this.End.Value
			: index.Value >= @this.Start.Value && index.Value < @this.End.Value;

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static bool Contains(this Range @this, Range other) => other.IsReverse() switch
	{
		_ when !@this.Any() && !other.Any() => false,
		false => @this.Contains(other.Start) && @this.Contains(other.End.Previous()),
		_ => @this.Contains(other.Start.Previous()) && @this.Contains(other.End)
	};

	/// <summary>
	/// <c>=&gt; (@<paramref name="this"/>.End.Value - @<paramref name="this"/>.Start.Value).AbsoluteValue();</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int Count(this Range @this)
		=> (@this.End.Value - @this.Start.Value).Abs();

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach(this Range @this, Action<int> action)
	{
		action.ThrowIfNull();

		foreach (var i in @this)
			action(i);
	}

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Start.FromStart(<paramref name="count"/>)..@<paramref name="this"/>.End.FromStart(<paramref name="count"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException"/>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Range FromStart(this Range @this, int count)
		=> @this.Start.FromStart(count)..@this.End.FromStart(count);

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static IEnumerator<int> GetEnumerator(this Range @this)
		=> !@this.IsReverse() ? @this.ToEnumerable().GetEnumerator() : @this.ToEnumerable().Reverse().GetEnumerator();

	/// <remarks>
	/// Reversal can only be determined if both Range Indexes have the same <c>IsFromEnd</c> value.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static bool IsReverse(this Range @this)
	{
		(@this.Start.IsFromEnd == @this.End.IsFromEnd).ThrowIfFalse();

		return @this.Start.IsFromEnd ? @this.Start.Value <= @this.End.Value : @this.Start.Value > @this.End.Value;
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static Index Maximum(this Range @this)
	{
		@this.Start.IsFromEnd.ThrowIfTrue();
		@this.End.IsFromEnd.ThrowIfTrue();
		@this.Start.ThrowIfEqual(@this.End);

		return @this.IsReverse() ? @this.Start : @this.End.Previous();
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static Index Minimum(this Range @this)
	{
		@this.Start.IsFromEnd.ThrowIfTrue();
		@this.End.IsFromEnd.ThrowIfTrue();
		@this.Start.ThrowIfEqual(@this.End);

		return @this.IsReverse() ? @this.End.Next() : @this.Start;
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static bool Overlaps(this Range @this, Range other)
		=> other.IsReverse()
			? @this.Contains(other.Start.Next()) || @this.Contains(other.End)
			: @this.Contains(other.Start) || @this.Contains(other.End.Previous());

	/// <summary>
	/// <c>=&gt; <see langword="new"/> <see cref="Range"/>(@<paramref name="this"/>.End, @<paramref name="this"/>.Start);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Range Reverse(this Range @this)
		=> new(@this.End, @this.Start);

	/// <exception cref="ArgumentOutOfRangeException"/>
	[DebuggerHidden]
	public static IEnumerable<int> ToEnumerable(this Range @this)
		=> !@this.IsReverse()
			? Enumerable.Range(@this.Start.Value, @this.Count())
			: Enumerable.Range(@this.End.Value, @this.Count()).Reverse();
}
