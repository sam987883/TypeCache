// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class IndexExtensions
{
	/// <inheritdoc cref="Index.Index(int, bool)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.IsFromEnd ?  (<paramref name="count"/> &gt; 0 ? <see langword="new"/> <see cref="Index"/>(<paramref name="count"/> - @<paramref name="this"/>.Value) : <see langword="default"/>) : @<paramref name="this"/>;</c>
	/// </remarks>
	public static Index FromStart(this Index @this, int count)
		=> @this.IsFromEnd ? (count > 0 ? new Index(count - @this.Value) : default) : @this;

	/// <inheritdoc cref="Index.Index(int, bool)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="Index"/>(@<paramref name="this"/>.Value + <paramref name="increment"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Index Next(this Index @this, int increment = 1)
		=> new Index(@this.Value + increment);

	/// <inheritdoc cref="Index.Index(int, bool)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="Index"/>(@<paramref name="this"/>.Value - <paramref name="increment"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static Index Previous(this Index @this, int increment = 1)
		=> new Index(@this.Value - increment);
}
