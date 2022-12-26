// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public static class IndexExtensions
{
	public static Index FromStart(this Index @this, int count)
		=> @this.IsFromEnd ? (count > 0 ? new Index(count - @this.Value) : default) : @this;

	/// <inheritdoc cref="Index.Index(int, bool)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="Index"/>(@<paramref name="this"/>.Value + <paramref name="increment"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Index Next(this Index @this, int increment = 1)
		=> new Index(@this.Value + increment);

	/// <inheritdoc cref="Index.Index(int, bool)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="Index"/>(@<paramref name="this"/>.Value - <paramref name="increment"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Index Previous(this Index @this, int increment = 1)
		=> new Index(@this.Value - increment);
}
