// Copyright (c) 2021 Samuel Abraham

using static System.TimeZoneInfo;

namespace TypeCache.Extensions;

public static class DateTimeExtensions
{
	/// <inheritdoc cref="DateTime.SpecifyKind(DateTime, DateTimeKind)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="DateTime"/>.SpecifyKind(@<paramref name="this"/>, <paramref name="kind"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static DateTime As(this DateTime @this, DateTimeKind kind)
		=> DateTime.SpecifyKind(@this, kind);

	/// <inheritdoc cref="TimeZoneInfo.ConvertTime(DateTime, TimeZoneInfo, TimeZoneInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTime(@<paramref name="this"/>, <paramref name="sourceTimeZone"/>, <paramref name="targetTimeZone"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static DateTime ChangeTimeZone(this DateTime @this, TimeZoneInfo sourceTimeZone, TimeZoneInfo targetTimeZone)
		=> ConvertTime(@this, sourceTimeZone, targetTimeZone);

	/// <inheritdoc cref="TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime, string, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTimeBySystemTimeZoneId(@<paramref name="this"/>, <paramref name="sourceSystemTimeZoneId"/>, <paramref name="targetSystemTimeZoneId"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static DateTime ChangeTimeZone(this DateTime @this, string sourceSystemTimeZoneId, string targetSystemTimeZoneId)
		=> ConvertTimeBySystemTimeZoneId(@this, sourceSystemTimeZoneId, targetSystemTimeZoneId);

	/// <inheritdoc cref="DateOnly.FromDateTime(DateTime)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="DateOnly"/>.FromDateTime(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static DateOnly ToDateOnly(this DateTime @this)
		=> DateOnly.FromDateTime(@this);

	/// <inheritdoc cref="DateOnly.FromDateTime(DateTime)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="DateOnly"/>.FromDateTime(@<paramref name="this"/>.DateTime);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static DateOnly ToDateOnly(this DateTimeOffset @this)
		=> DateOnly.FromDateTime(@this.DateTime);

	/// <inheritdoc cref="DateTimeOffset.DateTimeOffset(DateTime)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="DateTimeOffset"/>(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static DateTimeOffset ToDateTimeOffset(this DateTime @this)
		=> new(@this);

	/// <inheritdoc cref="DateTimeOffset.DateTimeOffset(DateTime, TimeSpan)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="DateTimeOffset"/>(@<paramref name="this"/>, <paramref name="offset"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static DateTimeOffset ToDateTimeOffset(this DateTime @this, TimeSpan offset)
		=> new(@this, offset);

	/// <inheritdoc cref="TimeOnly.FromDateTime(DateTime)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeOnly"/>.FromDateTime(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TimeOnly ToTimeOnly(this DateTime @this)
		=> TimeOnly.FromDateTime(@this);

	/// <inheritdoc cref="TimeOnly.FromDateTime(DateTime)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeOnly"/>.FromDateTime(@<paramref name="this"/>.DateTime);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TimeOnly ToTimeOnly(this DateTimeOffset @this)
		=> TimeOnly.FromDateTime(@this.DateTime);

	/// <exception cref="UnreachableException"/>
	public static DateTime ToTimeZone(this DateTime @this, DateTimeKind kind)
		=> kind switch
		{
			_ when kind == @this.Kind => @this,
			DateTimeKind.Local => ConvertTimeFromUtc(@this, TimeZoneInfo.Local),
			DateTimeKind.Unspecified => @this.As(kind),
			DateTimeKind.Utc => ConvertTimeToUtc(@this),
			_ => throw new UnreachableException($"{nameof(ToTimeZone)}: {nameof(DateTimeKind)} value of {kind:D} is not supported.")
		};

	/// <exception cref="TimeZoneNotFoundException"/>
	public static DateTime ToTimeZone(this DateTime @this, TimeZoneInfo targetTimeZone)
		=> @this.Kind switch
		{
			DateTimeKind.Local => ConvertTime(@this, targetTimeZone),
			DateTimeKind.Utc => ConvertTimeFromUtc(@this, targetTimeZone),
			_ => throw new TimeZoneNotFoundException($"{nameof(ConvertTime)}: {nameof(DateTime)} value must have a {nameof(DateTimeKind)} of {DateTimeKind.Local} or {DateTimeKind.Utc} only.")
		};

	/// <inheritdoc cref="TimeZoneInfo.ConvertTime(DateTimeOffset, TimeZoneInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTime(@<paramref name="this"/>, <paramref name="targetTimeZone"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static DateTimeOffset ToTimeZone(this DateTimeOffset @this, TimeZoneInfo targetTimeZone)
		=> ConvertTime(@this, targetTimeZone);

	/// <inheritdoc cref="TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTimeOffset, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTimeBySystemTimeZoneId(@<paramref name="this"/>, <paramref name="targetSystemTimeZoneId"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static DateTimeOffset ToTimeZone(this DateTimeOffset @this, string targetSystemTimeZoneId)
		=> ConvertTimeBySystemTimeZoneId(@this, targetSystemTimeZoneId);

	/// <inheritdoc cref="TimeZoneInfo.ConvertTimeToUtc(DateTime)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTimeToUtc(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static DateTime ToUTC(this DateTime @this)
		=> ConvertTimeToUtc(@this);

	/// <inheritdoc cref="TimeZoneInfo.ConvertTimeToUtc(DateTime, TimeZoneInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTimeToUtc(@<paramref name="this"/>, <paramref name="sourceTimeZone"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static DateTime ToUTC(this DateTime @this, TimeZoneInfo sourceTimeZone)
		=> ConvertTimeToUtc(@this, sourceTimeZone);
}
