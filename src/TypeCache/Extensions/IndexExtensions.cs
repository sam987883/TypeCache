// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public static class IndexExtensions
{
	extension(Index @this)
	{
		public Index FromStart(int count)
			=> @this.IsFromEnd ? (count > 0 ? new Index(count - @this.Value) : default) : @this;

		/// <inheritdoc cref="Index.Index(int, bool)"/>
		/// <remarks>
		/// <c>=&gt; <see langword="new"/> <see cref="Index"/>(@this.Value + <paramref name="increment"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Index Next(int increment = 1)
			=> new(@this.Value + increment);

		/// <inheritdoc cref="Index.Index(int, bool)"/>
		/// <remarks>
		/// <c>=&gt; <see langword="new"/> <see cref="Index"/>(@this.Value - <paramref name="increment"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public Index Previous(int increment = 1)
			=> new(@this.Value - increment);
	}

	extension((Index, Index) @this)
	{
		/// <exception cref="ArgumentOutOfRangeException"/>
		public Index Max()
		{
			(@this.Item1.IsFromEnd == @this.Item2.IsFromEnd).ThrowIfFalse();

			return @this.Item1.IsFromEnd
				? Index.FromEnd((@this.Item1.Value, @this.Item2.Value).Min())
				: Index.FromStart((@this.Item1.Value, @this.Item2.Value).Max());
		}

		/// <exception cref="ArgumentOutOfRangeException"/>
		public Index Min()
		{
			(@this.Item1.IsFromEnd == @this.Item2.IsFromEnd).ThrowIfFalse();

			return @this.Item1.IsFromEnd
				? Index.FromEnd((@this.Item1.Value, @this.Item2.Value).Max())
				: Index.FromStart((@this.Item1.Value, @this.Item2.Value).Min());
		}
	}
}
