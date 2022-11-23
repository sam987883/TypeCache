// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;
using static System.TimeZoneInfo;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class DateTimeExtensions
{
	/// <inheritdoc cref="DateTime.SpecifyKind(DateTime, DateTimeKind)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="DateTime"/>.SpecifyKind(@<paramref name="this"/>, <paramref name="kind"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static DateTime As(this DateTime @this, DateTimeKind kind)
		=> DateTime.SpecifyKind(@this, kind);

	/// <inheritdoc cref="TimeZoneInfo.ConvertTime(DateTime, TimeZoneInfo, TimeZoneInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTime(@<paramref name="this"/>, <paramref name="sourceTimeZone"/>, <paramref name="targetTimeZone"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static DateTime ChangeTimeZone(this DateTime @this, TimeZoneInfo sourceTimeZone, TimeZoneInfo targetTimeZone)
		=> ConvertTime(@this, sourceTimeZone, targetTimeZone);

	/// <inheritdoc cref="TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime, string, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTimeBySystemTimeZoneId(@<paramref name="this"/>, <paramref name="sourceSystemTimeZoneId"/>, <paramref name="targetSystemTimeZoneId"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static DateTime ChangeTimeZone(this DateTime @this, string sourceSystemTimeZoneId, string targetSystemTimeZoneId)
		=> ConvertTimeBySystemTimeZoneId(@this, sourceSystemTimeZoneId, targetSystemTimeZoneId);

	/// <remarks>
	/// <code>
	/// =&gt; <paramref name="kind"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    "/>_ <see langword="when"/> <paramref name="kind"/> == @<paramref name="this"/>.Kind =&gt; @<paramref name="this"/>,<br/>
	/// <see langword="    "/><see cref="DateTimeKind.Local"/> =&gt; <see cref="TimeZoneInfo"/>.ConvertTimeFromUtc(@<paramref name="this"/>, <see cref="TimeZoneInfo.Local"/>),<br/>
	/// <see langword="    "/><see cref="DateTimeKind.Unspecified"/> =&gt; @<paramref name="this"/>.As(<paramref name="kind"/>),<br/>
	/// <see langword="    "/><see cref="DateTimeKind.Utc"/> =&gt; <see cref="TimeZoneInfo"/>.ConvertTimeToUtc(@<paramref name="this"/>),<br/>
	/// <see langword="    "/>_ =&gt; <see langword="throw new"/> <see cref="UnreachableException"/>($"{<see langword="nameof"/>(ToTimeZone)}: {<see langword="nameof"/>(<see cref="DateTimeKind"/>)} value of {<paramref name="kind"/>} is not supported.")<br/>
	/// };
	/// </code>
	/// </remarks>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
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

	/// <remarks>
	/// <code>
	/// =&gt; @<paramref name="this"/>.Kind <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    "/><see cref="DateTimeKind.Local"/> =&gt; <see cref="TimeZoneInfo"/>.ConvertTime(@<paramref name="this"/>, <paramref name="targetTimeZone"/>),<br/>
	/// <see langword="    "/><see cref="DateTimeKind.Utc"/> =&gt; <see cref="TimeZoneInfo"/>.ConvertTimeFromUtc(@<paramref name="this"/>, <paramref name="targetTimeZone"/>),<br/>
	/// <see langword="    "/>_ =&gt; <see langword="throw new"/> <see cref="TimeZoneNotFoundException"/>($"{<see langword="nameof"/>(ConvertTime)}: {<see langword="nameof"/>(<see cref="DateTime"/>)} value must have a {<see langword="nameof"/>(<see cref="DateTimeKind"/>)} of {<see cref="DateTimeKind.Local"/>} or {<see cref="DateTimeKind.Utc"/>}.")<br/>
	/// };
	/// </code>
	/// </remarks>
	/// <exception cref="TimeZoneNotFoundException">if: <c>@<paramref name="this"/>.Kind = <see cref="DateTimeKind.Unspecified"/></c></exception>
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
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static DateTimeOffset ToTimeZone(this DateTimeOffset @this, TimeZoneInfo targetTimeZone)
		=> ConvertTime(@this, targetTimeZone);

	/// <inheritdoc cref="TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTimeOffset, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTimeBySystemTimeZoneId(@<paramref name="this"/>, <paramref name="targetSystemTimeZoneId"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static DateTimeOffset ToTimeZone(this DateTimeOffset @this, string targetSystemTimeZoneId)
		=> TimeZoneInfo.ConvertTimeBySystemTimeZoneId(@this, targetSystemTimeZoneId);

	/// <inheritdoc cref="TimeZoneInfo.ConvertTimeToUtc(DateTime)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTimeToUtc(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static DateTime ToUTC(this DateTime @this)
		=> ConvertTimeToUtc(@this);

	/// <inheritdoc cref="TimeZoneInfo.ConvertTimeToUtc(DateTime, TimeZoneInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTimeToUtc(@<paramref name="this"/>, <paramref name="sourceTimeZone"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static DateTime ToUTC(this DateTime @this, TimeZoneInfo sourceTimeZone)
		=> ConvertTimeToUtc(@this, sourceTimeZone);
}
