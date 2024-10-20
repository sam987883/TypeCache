using System.Numerics;

namespace TypeCache.Extensions;

public static partial class NumericExtensions
{
	/// <inheritdoc cref="TimeSpan.FromDays(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeSpan"/>.FromDays(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TimeSpan ToDays(this double @this)
		=> TimeSpan.FromDays(@this);

	/// <inheritdoc cref="TimeSpan.FromHours(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeSpan"/>.FromHours(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TimeSpan ToHours(this double @this)
		=> TimeSpan.FromHours(@this);

	/// <inheritdoc cref="TimeSpan.FromMicroseconds(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeSpan"/>.FromMicroseconds(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TimeSpan ToMicroseconds(this double @this)
		=> TimeSpan.FromMicroseconds(@this);

	/// <inheritdoc cref="TimeSpan.FromMilliseconds(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeSpan"/>.FromMilliseconds(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TimeSpan ToMilliseconds(this double @this)
		=> TimeSpan.FromMilliseconds(@this);

	/// <inheritdoc cref="TimeSpan.FromMinutes(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeSpan"/>.FromMinutes(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TimeSpan ToMinutes(this double @this)
		=> TimeSpan.FromMinutes(@this);

	/// <inheritdoc cref="TimeSpan.FromSeconds(double)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeSpan"/>.FromSeconds(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TimeSpan ToSeconds(this double @this)
		=> TimeSpan.FromSeconds(@this);
}
