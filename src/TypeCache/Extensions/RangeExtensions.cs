// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public static class RangeExtensions
{
	extension(Range @this)
	{
		/// <summary>
		/// <c>=&gt; (@this.End.Value - @this.Start.Value).AbsoluteValue();</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public int Count()
			=> (@this.End.Value - @this.Start.Value).Abs();

		/// <exception cref="ArgumentOutOfRangeException"/>
		public bool Any()
		{
			(@this.Start.IsFromEnd == @this.End.IsFromEnd).ThrowIfFalse();

			return !@this.Start.Equals(@this.End);
		}

		/// <exception cref="ArgumentOutOfRangeException"/>
		public bool Contains(Index index)
			=> @this.IsReverse()
				? index.Value < @this.Start.Value && index.Value >= @this.End.Value
				: index.Value >= @this.Start.Value && index.Value < @this.End.Value;

		/// <exception cref="ArgumentOutOfRangeException"/>
		public bool Contains(Range other)
			=> other.IsReverse() switch
			{
				_ when !@this.Any() && !other.Any() => false,
				false => @this.Contains(other.Start) && @this.Contains(other.End.Previous()),
				_ => @this.Contains(other.Start.Previous()) && @this.Contains(other.End)
			};

		/// <exception cref="ArgumentNullException"/>
		public void ForEach(Action<int> action)
		{
			action.ThrowIfNull();

			foreach (var i in @this)
				action(i);
		}

		/// <summary>
		/// <c>=&gt; @this.Start.FromStart(<paramref name="count"/>)..@this.End.FromStart(<paramref name="count"/>);</c>
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"/>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Range FromStart(int count)
			=> @this.Start.FromStart(count)..@this.End.FromStart(count);

		/// <exception cref="ArgumentOutOfRangeException"/>
		public IEnumerator<int> GetEnumerator()
			=> !@this.IsReverse() ? @this.ToEnumerable().GetEnumerator() : @this.ToEnumerable().Reverse().GetEnumerator();

		/// <remarks>
		/// Reversal can only be determined if both Range Indexes have the same <c>IsFromEnd</c> value.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public bool IsReverse()
		{
			(@this.Start.IsFromEnd == @this.End.IsFromEnd).ThrowIfFalse();

			return @this.Start.IsFromEnd ? @this.Start.Value <= @this.End.Value : @this.Start.Value > @this.End.Value;
		}

		/// <exception cref="ArgumentOutOfRangeException"/>
		public Index Maximum()
		{
			@this.Start.IsFromEnd.ThrowIfTrue();
			@this.End.IsFromEnd.ThrowIfTrue();
			@this.Start.ThrowIfEqual(@this.End);

			return @this.IsReverse() ? @this.Start : @this.End.Previous();
		}

		/// <exception cref="ArgumentOutOfRangeException"/>
		public Index Minimum()
		{
			@this.Start.IsFromEnd.ThrowIfTrue();
			@this.End.IsFromEnd.ThrowIfTrue();
			@this.Start.ThrowIfEqual(@this.End);

			return @this.IsReverse() ? @this.End.Next() : @this.Start;
		}

		/// <exception cref="ArgumentOutOfRangeException"/>
		public bool Overlaps(Range other)
			=> other.IsReverse()
				? @this.Contains(other.Start.Next()) || @this.Contains(other.End)
				: @this.Contains(other.Start) || @this.Contains(other.End.Previous());

		/// <summary>
		/// <c>=&gt; <see langword="new"/> <see cref="Range"/>(@this.End, @this.Start);</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Range Reverse()
			=> new(@this.End, @this.Start);

		/// <exception cref="ArgumentOutOfRangeException"/>
		[DebuggerHidden]
		public IEnumerable<int> ToEnumerable()
			=> !@this.IsReverse()
				? Enumerable.Range(@this.Start.Value, @this.Count())
				: Enumerable.Range(@this.End.Value, @this.Count()).Reverse();
	}
}
