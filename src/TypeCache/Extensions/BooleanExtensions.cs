// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public static class BooleanExtensions
{
	[DebuggerHidden]
	public static bool Else(this bool @this, Action doIfFalse)
	{
		if (!@this)
			doIfFalse();

		return @this;
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T If<T>(this bool @this, T trueValue, T falseValue)
		=> @this ? trueValue : falseValue;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T If<T>(this bool @this, Func<T> trueValue, T falseValue)
		=> @this ? trueValue() : falseValue;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T If<T>(this bool @this, Func<T> trueValue, Func<T> falseValue)
		=> @this ? trueValue() : falseValue();

	[DebuggerHidden]
	public static bool Then(this bool @this, Action doIfTrue)
	{
		if (@this)
			doIfTrue();

		return @this;
	}

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/> ? [(<see cref="byte"/>)1] : [(<see cref="byte"/>)0];</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] ToBytes(this bool @this)
		=> @this ? [(byte)1] : [(byte)0];
}
