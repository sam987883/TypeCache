using System.Numerics;
using System.Runtime.InteropServices;

namespace TypeCache.Extensions;

public static partial class NumericExtensions
{
	/// <inheritdoc cref="INumber{TSelf}.Max(TSelf, TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Max(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Max<T>(this (T, T) @this)
		where T : INumber<T>
		=> T.Max(@this.Item1, @this.Item2);

	/// <inheritdoc cref="INumber{TSelf}.Min(TSelf, TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Min(@<paramref name="this"/>.Item1, @<paramref name="this"/>.Item2);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static T Min<T>(this (T, T) @this)
		where T : INumber<T>
		=> T.Min(@this.Item1, @this.Item2);

	/// <inheritdoc cref="INumber{TSelf}.Sign(TSelf)"/>
	/// <remarks>
	/// <c>=&gt; <typeparamref name="T"/>.Sign(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static int Sign<T>(this T @this)
		where T : INumber<T>
		=> T.Sign(@this);

	public static byte[] ToBytes<T>(this T @this)
		where T : struct, INumber<T>
		=> @this switch
		{
			char value => BitConverter.GetBytes(value),
			sbyte value => [(byte)value],
			short value => BitConverter.GetBytes(value),
			int value => BitConverter.GetBytes(value),
			nint value => BitConverter.GetBytes(value),
			long value => BitConverter.GetBytes(value),
			byte value => [value],
			ushort value => BitConverter.GetBytes(value),
			uint value => BitConverter.GetBytes(value),
			nuint value => BitConverter.GetBytes(value),
			ulong value => BitConverter.GetBytes(value),
			BigInteger value => value.ToByteArray(),
			Half value => BitConverter.GetBytes(value),
			float value => BitConverter.GetBytes(value),
			double value => BitConverter.GetBytes(value),
			decimal value => decimal.GetBits(value).SelectMany(BitConverter.GetBytes).ToArray(),
			NFloat value => BitConverter.GetBytes(value),
			_ => throw new UnreachableException(Invariant($"Cannot convert [{@this.GetType().FullName}] to bytes."))
		};
}
