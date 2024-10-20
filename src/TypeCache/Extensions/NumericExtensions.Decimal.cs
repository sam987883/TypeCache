using System.Numerics;

namespace TypeCache.Extensions;

public static partial class NumericExtensions
{
	/// <inheritdoc cref="decimal.GetBits(decimal)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="decimal"/>.GetBits(@<paramref name="this"/>).SelectMany(_ =&gt; _.GetBytes()).ToArray();</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static byte[] ToBytes(this decimal @this)
		=> decimal.GetBits(@this).SelectMany(_ => _.GetBytes()).ToArray();
}
