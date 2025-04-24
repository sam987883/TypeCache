// Copyright (c) 2021 Samuel Abraham

using System.Numerics;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions;

public static partial class NumericExtensions
{
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static ulong Factorial(this int @this)
	{
		(@this < 0).ThrowIfTrue();

		if (@this is 0)
			return 1UL;

		var result = (ulong)@this;
		while (@this > 1)
			result *= (ulong)(--@this);

		return result;
	}

	/// <inheritdoc cref="BitConverter.Int32BitsToSingle(int)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Unsafe"/>.BitCast&lt;<see cref="int"/>, <see cref="float"/>&gt;(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static float ToSingle(this int @this)
		=> Unsafe.BitCast<int, float>(@this);
}
