// Copyright (c) 2021 Samuel Abraham

using static System.Math;

namespace TypeCache.Extensions;

public static class RangeExtensions
{
	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Start.IsFromEnd == @<paramref name="this"/>.End.IsFromEnd
	/// ? !@<paramref name="this"/>.Start.Equals(@<paramref name="this"/>.End)
	/// : <see langword="null"/>;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool? Any(this Range @this)
		=> @this.Start.IsFromEnd == @this.End.IsFromEnd ? !@this.Start.Equals(@this.End) : null;

	/// <exception cref="ArgumentNullException"/>
	public static void ForEach(this Range @this, Action<int> action)
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

	public static bool Has(this Range @this, Index index)
		=> @this.IsReverse() switch
		{
			true => index.Value <= @this.Start.Value && index.Value > @this.End.Value,
			false => index.Value >= @this.Start.Value && index.Value < @this.End.Value,
			_ => false
		};

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

	public static Range? IntersectWith(this Range @this, Range other)
		=> @this.Overlaps(other) ? (@this.IsReverse(), other.IsReverse()) switch
		{
			(true, true) => (@this.Start, other.Start).Minimum()..(@this.End, other.End).Maximum(),
			(true, false) => (@this.Start, other.End.Previous()).Minimum()..(@this.End, other.Start.Next()).Maximum(),
			(false, true) => (@this.Start, other.End.Next()).Maximum()..(@this.End, other.Start.Previous()).Minimum(),
			(false, false) => (@this.Start, other.Start).Maximum()..(@this.End, other.End).Minimum(),
			_ => null
		} : null;

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
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int Length(this Range @this)
		=> Abs(@this.End.Value - @this.Start.Value);

	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<T> Select<T>(this Range @this, Func<int, T> map)
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

	public static Index? Maximum(this Range @this)
		=> @this.IsReverse() switch
		{
			true when @this.Start.Value > @this.End.Value => @this.Start,
			false when @this.Start.Value < @this.End.Value => @this.End.Previous(),
			_ => null
		};

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
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Range Reverse(this Range @this)
		=> new Range(@this.End, @this.Start);

	public static Range? UnionWith(this Range @this, Range other)
		=> @this.Has(other.Start) || @this.Has(other.End) || other.Has(@this.Start) || other.Has(@this.End) ? (@this.IsReverse(), other.IsReverse()) switch
		{
			(true, true) => (@this.Start, other.Start).Maximum()..(@this.End, other.End).Minimum(),
			(true, false) => (@this.Start, other.End.Previous()).Maximum()..(@this.End, other.Start.Next()).Minimum(),
			(false, true) => (@this.Start, other.End.Next()).Minimum()..(@this.End, other.Start.Previous()).Maximum(),
			(false, false) => (@this.Start, other.Start).Minimum()..(@this.End, other.End).Maximum(),
			_ => null
		} : null;

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

	/// <exception cref="ArgumentNullException"/>
	public static IEnumerable<int> Where(this Range @this, Predicate<int> condition)
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
}
