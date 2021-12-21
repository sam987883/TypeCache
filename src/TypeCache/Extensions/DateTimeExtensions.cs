// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using System.Security;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class DateTimeExtensions
{
	/// <summary>
	/// <c>=&gt; <see cref="DateTime"/>.SpecifyKind(@<paramref name="this"/>, <paramref name="kind"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static DateTime As(this DateTime @this, DateTimeKind kind)
		=> DateTime.SpecifyKind(@this, kind);

	/// <summary>
	/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTime(@<paramref name="this"/>, <paramref name="sourceTimeZone"/>, <paramref name="targetTimeZone"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static DateTime ConvertTime(this DateTime @this, TimeZoneInfo sourceTimeZone, TimeZoneInfo targetTimeZone)
		=> TimeZoneInfo.ConvertTime(@this, sourceTimeZone, targetTimeZone);

	/// <summary>
	/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTimeBySystemTimeZoneId(@<paramref name="this"/>, <paramref name="sourceSystemTimeZoneId"/>, <paramref name="targetSystemTimeZoneId"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="InvalidTimeZoneException"/>
	/// <exception cref="SecurityException"/>
	/// <exception cref="TimeZoneNotFoundException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static DateTime ConvertTime(this DateTime @this, string sourceSystemTimeZoneId, string targetSystemTimeZoneId)
		=> TimeZoneInfo.ConvertTimeBySystemTimeZoneId(@this, sourceSystemTimeZoneId, targetSystemTimeZoneId);

	/// <summary>
	/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTimeToUtc(@<paramref name="this"/>, <paramref name="sourceTimeZone"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static DateTime ConvertTimeToUTC(this DateTime @this, TimeZoneInfo sourceTimeZone)
		=> TimeZoneInfo.ConvertTimeToUtc(@this, sourceTimeZone);

	/// <summary>
	/// <code>
	/// =&gt; <paramref name="kind"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    "/>_ <see langword="when"/> <paramref name="kind"/> == @<paramref name="this"/>.Kind =&gt; @<paramref name="this"/>,<br/>
	/// <see langword="    "/><see cref="DateTimeKind.Local"/> =&gt; <see cref="TimeZoneInfo"/>.ConvertTimeFromUtc(@<paramref name="this"/>, <see cref="TimeZoneInfo.Local"/>),<br/>
	/// <see langword="    "/><see cref="DateTimeKind.Unspecified"/> =&gt; @<paramref name="this"/>.As(<paramref name="kind"/>),<br/>
	/// <see langword="    "/><see cref="DateTimeKind.Utc"/> =&gt; <see cref="TimeZoneInfo"/>.ConvertTimeToUtc(@<paramref name="this"/>),<br/>
	/// <see langword="    "/>_ =&gt; <see langword="throw new"/> <see cref="ArgumentOutOfRangeException"/>($"{<see langword="nameof"/>(To)}: {<see langword="nameof"/>(<see cref="DateTimeKind"/>)} value of {<paramref name="kind"/>} is not supported.")<br/>
	/// };
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static DateTime To(this DateTime @this, DateTimeKind kind)
		=> kind switch
		{
			_ when kind == @this.Kind => @this,
			DateTimeKind.Local => TimeZoneInfo.ConvertTimeFromUtc(@this, TimeZoneInfo.Local),
			DateTimeKind.Unspecified => @this.As(kind),
			DateTimeKind.Utc => TimeZoneInfo.ConvertTimeToUtc(@this),
			_ => throw new ArgumentOutOfRangeException($"{nameof(To)}: {nameof(DateTimeKind)} value of {kind} is not supported.")
		};

	/// <summary>
	/// <code>
	/// =&gt; <paramref name="kind"/> <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    "/><see cref="DateTimeKind.Local"/> =&gt; <see cref="TimeZoneInfo"/>.ConvertTime(@<paramref name="this"/>, <paramref name="targetTimeZone"/>),<br/>
	/// <see langword="    "/><see cref="DateTimeKind.Utc"/> =&gt; <see cref="TimeZoneInfo"/>.ConvertTimeFromUtc(@<paramref name="this"/>, <paramref name="targetTimeZone"/>),<br/>
	/// <see langword="    "/>_ =&gt; <see langword="throw new"/> <see cref="TimeZoneNotFoundException"/>($"{<see langword="nameof"/>(To)}: {<see langword="nameof"/>(<see cref="DateTime"/>)} value must have a {<see langword="nameof"/>(<see cref="DateTimeKind"/>)} of {<see cref="DateTimeKind.Local"/>} or {<see cref="DateTimeKind.Utc"/>}.")<br/>
	/// };
	/// </code>
	/// </summary>
	/// <exception cref="TimeZoneNotFoundException"/>
	public static DateTime To(this DateTime @this, TimeZoneInfo targetTimeZone)
		=> @this.Kind switch
		{
			DateTimeKind.Local => TimeZoneInfo.ConvertTime(@this, targetTimeZone),
			DateTimeKind.Utc => TimeZoneInfo.ConvertTimeFromUtc(@this, targetTimeZone),
			_ => throw new TimeZoneNotFoundException($"{nameof(ConvertTime)}: {nameof(DateTime)} value must have a {nameof(DateTimeKind)} of {DateTimeKind.Local} or {DateTimeKind.Utc}.")
		};

	/// <summary>
	/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTime(@<paramref name="this"/>, <paramref name="targetTimeZone"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static DateTimeOffset To(this DateTimeOffset @this, TimeZoneInfo targetTimeZone)
		=> TimeZoneInfo.ConvertTime(@this, targetTimeZone);

	/// <summary>
	/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTimeBySystemTimeZoneId(@<paramref name="this"/>, <paramref name="targetSystemTimeZoneId"/>);</c>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="InvalidTimeZoneException"/>
	/// <exception cref="SecurityException"/>
	/// <exception cref="TimeZoneNotFoundException"/>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static DateTimeOffset To(this DateTimeOffset @this, string targetSystemTimeZoneId)
		=> TimeZoneInfo.ConvertTimeBySystemTimeZoneId(@this, targetSystemTimeZoneId);
}
