using System.Numerics;

namespace TypeCache.Extensions;

public static partial class NumericExtensions
{
	/// <inheritdoc cref="IFloatingPointIeee754{TSelf}.BitDecrement(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.BitDecrement(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T BitDecrement<T>(this T @this)
		where T : IFloatingPointIeee754<T>
		=> T.BitDecrement(@this);

	/// <inheritdoc cref="IFloatingPointIeee754{TSelf}.BitIncrement(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.BitIncrement(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T BitIncrement<T>(this T @this)
		where T : IFloatingPointIeee754<T>
		=> T.BitIncrement(@this);
}
