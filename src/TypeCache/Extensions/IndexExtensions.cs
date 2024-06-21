// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public static class IndexExtensions
{
	public static Index FromStart(this Index @this, int count)
		=> @this.IsFromEnd ? (count > 0 ? new Index(count - @this.Value) : default) : @this;

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static Index Max(this (Index, Index) @this)
	{
		(@this.Item1.IsFromEnd == @this.Item2.IsFromEnd).ThrowIfFalse();

		return @this.Item1.IsFromEnd
			? Index.FromEnd((@this.Item1.Value, @this.Item2.Value).Min())
			: Index.FromStart((@this.Item1.Value, @this.Item2.Value).Max());
	}

	/// <exception cref="ArgumentOutOfRangeException"/>
	public static Index Min(this (Index, Index) @this)
	{
		(@this.Item1.IsFromEnd == @this.Item2.IsFromEnd).ThrowIfFalse();

		return @this.Item1.IsFromEnd
			? Index.FromEnd((@this.Item1.Value, @this.Item2.Value).Max())
			: Index.FromStart((@this.Item1.Value, @this.Item2.Value).Min());
	}

	/// <inheritdoc cref="Index.Index(int, bool)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="Index"/>(@<paramref name="this"/>.Value + <paramref name="increment"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Index Next(this Index @this, int increment = 1)
		=> new(@this.Value + increment);

	/// <inheritdoc cref="Index.Index(int, bool)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="Index"/>(@<paramref name="this"/>.Value - <paramref name="increment"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Index Previous(this Index @this, int increment = 1)
		=> new(@this.Value - increment);
}
