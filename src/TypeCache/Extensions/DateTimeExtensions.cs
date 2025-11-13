// Copyright (c) 2021 Samuel Abraham

using System.Globalization;
using static System.Globalization.CultureInfo;
using static System.TimeZoneInfo;

namespace TypeCache.Extensions;

public static class DateTimeExtensions
{
	extension(DateOnly @this)
	{
		/// <remarks>
		/// <c>=&gt; @this &gt;= <paramref name="minimum"/> &amp;&amp; @this &lt;= <paramref name="maximum"/>;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool Between(DateOnly minimum, DateOnly maximum)
			=> @this >= minimum && @this <= maximum;

		/// <remarks>
		/// <c>=&gt; @this &gt; <paramref name="minimum"/> &amp;&amp; @this &lt; <paramref name="maximum"/>;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool InBetween(DateOnly minimum, DateOnly maximum)
			=> @this > minimum && @this < maximum;

		/// <remarks>
		/// <c>=&gt; @this.ToString("o", <paramref name="provider"/> ?? <see cref="InvariantCulture"/>);</c>
		/// </remarks>
		/// <param name="provider">Defaults to <see cref="InvariantCulture"/>.</param>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToISO8601(IFormatProvider? provider = null)
			=> @this.ToString("o", provider ?? InvariantCulture);
	}

	extension(DateTime @this)
	{
		/// <inheritdoc cref="DateOnly.FromDateTime(DateTime)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="DateOnly"/>.FromDateTime(@this);</c>
		/// </remarks>
		[DebuggerHidden]
		public DateOnly DateOnly => DateOnly.FromDateTime(@this);

		/// <inheritdoc cref="TimeOnly.FromDateTime(DateTime)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="TimeOnly"/>.FromDateTime(@this);</c>
		/// </remarks>
		[DebuggerHidden]
		public TimeOnly TimeOnly => TimeOnly.FromDateTime(@this);

		/// <inheritdoc cref="DateTime.SpecifyKind(DateTime, DateTimeKind)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="DateTime"/>.SpecifyKind(@this, <paramref name="kind"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public DateTime As(DateTimeKind kind)
			=> DateTime.SpecifyKind(@this, kind);

		/// <remarks>
		/// <c>=&gt; @this &gt;= <paramref name="minimum"/> &amp;&amp; @this &lt;= <paramref name="maximum"/>;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool Between(DateTime minimum, DateTime maximum)
			=> @this >= minimum && @this <= maximum;

		/// <inheritdoc cref="TimeZoneInfo.ConvertTime(DateTime, TimeZoneInfo, TimeZoneInfo)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTime(@this, <paramref name="sourceTimeZone"/>, <paramref name="targetTimeZone"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public DateTime ChangeTimeZone(TimeZoneInfo sourceTimeZone, TimeZoneInfo targetTimeZone)
			=> ConvertTime(@this, sourceTimeZone, targetTimeZone);

		/// <inheritdoc cref="TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime, string, string)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTimeBySystemTimeZoneId(@this, <paramref name="sourceSystemTimeZoneId"/>, <paramref name="targetSystemTimeZoneId"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public DateTime ChangeTimeZone(string sourceSystemTimeZoneId, string targetSystemTimeZoneId)
			=> ConvertTimeBySystemTimeZoneId(@this, sourceSystemTimeZoneId, targetSystemTimeZoneId);

		/// <remarks>
		/// <c>=&gt; @this &gt; <paramref name="minimum"/> &amp;&amp; @this &lt; <paramref name="maximum"/>;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool InBetween(DateTime minimum, DateTime maximum)
			=> @this > minimum && @this < maximum;

		/// <remarks>
		/// <c>=&gt; @this.ToString("s", <paramref name="provider"/> ?? <see cref="InvariantCulture"/>);</c>
		/// </remarks>
		/// <param name="provider">Defaults to <see cref="InvariantCulture"/>.</param>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToISO8601(IFormatProvider? provider = null)
			=> @this.ToString("s", provider ?? InvariantCulture);

		/// <inheritdoc cref="DateTimeOffset.DateTimeOffset(DateTime)"/>
		/// <remarks>
		/// <c>=&gt; <see langword="new"/> <see cref="DateTimeOffset"/>(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public DateTimeOffset ToDateTimeOffset()
			=> new(@this);

		/// <inheritdoc cref="DateTimeOffset.DateTimeOffset(DateTime, TimeSpan)"/>
		/// <remarks>
		/// <c>=&gt; <see langword="new"/> <see cref="DateTimeOffset"/>(@this, <paramref name="offset"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public DateTimeOffset ToDateTimeOffset(TimeSpan offset)
			=> new(@this, offset);

		/// <exception cref="UnreachableException"/>
		public DateTime ToTimeZone(DateTimeKind kind)
			=> kind switch
			{
				_ when kind == @this.Kind => @this,
				DateTimeKind.Local => ConvertTimeFromUtc(@this, TimeZoneInfo.Local),
				DateTimeKind.Unspecified => @this.As(kind),
				DateTimeKind.Utc => ConvertTimeToUtc(@this),
				_ => throw new UnreachableException($"{nameof(ToTimeZone)}: {nameof(DateTimeKind)} value of {kind:D} is not supported.")
			};

		/// <exception cref="TimeZoneNotFoundException"/>
		public DateTime ToTimeZone(TimeZoneInfo targetTimeZone)
			=> @this.Kind switch
			{
				DateTimeKind.Local => ConvertTime(@this, targetTimeZone),
				DateTimeKind.Utc => ConvertTimeFromUtc(@this, targetTimeZone),
				_ => throw new TimeZoneNotFoundException($"{nameof(ConvertTime)}: {nameof(DateTime)} value must have a {nameof(DateTimeKind)} of {DateTimeKind.Local} or {DateTimeKind.Utc} only.")
			};

		/// <inheritdoc cref="TimeZoneInfo.ConvertTimeToUtc(DateTime)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTimeToUtc(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public DateTime ToUTC()
			=> ConvertTimeToUtc(@this);

		/// <inheritdoc cref="TimeZoneInfo.ConvertTimeToUtc(DateTime, TimeZoneInfo)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTimeToUtc(@this, <paramref name="sourceTimeZone"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public DateTime ToUTC(TimeZoneInfo sourceTimeZone)
			=> ConvertTimeToUtc(@this, sourceTimeZone);
	}

	extension(DateTimeOffset @this)
	{
		/// <inheritdoc cref="DateOnly.FromDateTime(DateTime)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="DateOnly"/>.FromDateTime(@this.DateTime);</c>
		/// </remarks>
		[DebuggerHidden]
		public DateOnly DateOnly => DateOnly.FromDateTime(@this.DateTime);

		/// <inheritdoc cref="TimeOnly.FromDateTime(DateTime)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="TimeOnly"/>.FromDateTime(@this.DateTime);</c>
		/// </remarks>
		[DebuggerHidden]
		public TimeOnly TimeOnly => TimeOnly.FromDateTime(@this.DateTime);

		/// <remarks>
		/// <c>=&gt; @this &gt;= <paramref name="minimum"/> &amp;&amp; @this &lt;= <paramref name="maximum"/>;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool Between(DateTimeOffset minimum, DateTimeOffset maximum)
			=> @this >= minimum && @this <= maximum;

		/// <remarks>
		/// <c>=&gt; @this &gt; <paramref name="minimum"/> &amp;&amp; @this &lt; <paramref name="maximum"/>;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool InBetween(DateTimeOffset minimum, DateTimeOffset maximum)
			=> @this > minimum && @this < maximum;

		/// <remarks>
		/// <c>=&gt; @this.ToString("yyyy-MM-dd'T'HH:mm:ssK", <paramref name="provider"/> ?? <see cref="InvariantCulture"/>);</c>
		/// </remarks>
		/// <param name="provider">Defaults to <see cref="InvariantCulture"/>.</param>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToISO8601(IFormatProvider? provider = null)
			=> @this.ToString("yyyy-MM-dd'T'HH:mm:ssK", provider ?? InvariantCulture);

		/// <inheritdoc cref="TimeZoneInfo.ConvertTime(DateTimeOffset, TimeZoneInfo)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTime(@this, <paramref name="targetTimeZone"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public DateTimeOffset ToTimeZone(TimeZoneInfo targetTimeZone)
			=> ConvertTime(@this, targetTimeZone);

		/// <inheritdoc cref="TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTimeOffset, string)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="TimeZoneInfo"/>.ConvertTimeBySystemTimeZoneId(@this, <paramref name="targetSystemTimeZoneId"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public DateTimeOffset ToTimeZone(string targetSystemTimeZoneId)
			=> ConvertTimeBySystemTimeZoneId(@this, targetSystemTimeZoneId);
	}

	extension(TimeOnly @this)
	{
		/// <remarks>
		/// <c>=&gt; @this &gt;= <paramref name="minimum"/> &amp;&amp; @this &lt;= <paramref name="maximum"/>;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool Between(TimeOnly minimum, TimeOnly maximum)
			=> @this >= minimum && @this <= maximum;

		/// <remarks>
		/// <c>=&gt; @this &gt; <paramref name="minimum"/> &amp;&amp; @this &lt; <paramref name="maximum"/>;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool InBetween(TimeOnly minimum, TimeOnly maximum)
			=> @this > minimum && @this < maximum;

		/// <remarks>
		/// <c>=&gt; @this.ToString("HH:mm:ss", <paramref name="provider"/> ?? <see cref="InvariantCulture"/>);</c>
		/// </remarks>
		/// <param name="provider">Defaults to <see cref="InvariantCulture"/>.</param>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToISO8601(IFormatProvider? provider = null)
			=> @this.ToString("HH:mm:ss", provider ?? InvariantCulture);
	}

	extension(TimeSpan @this)
	{
		/// <remarks>
		/// <c>=&gt; @this &gt;= <paramref name="minimum"/> &amp;&amp; @this &lt;= <paramref name="maximum"/>;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool Between(TimeSpan minimum, TimeSpan maximum)
			=> @this >= minimum && @this <= maximum;

		/// <remarks>
		/// <c>=&gt; @this &gt; <paramref name="minimum"/> &amp;&amp; @this &lt; <paramref name="maximum"/>;</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public bool InBetween(TimeSpan minimum, TimeSpan maximum)
			=> @this > minimum && @this < maximum;

		/// <remarks>
		/// <c>=&gt; @this.ToString("c", <paramref name="provider"/> ?? <see cref="InvariantCulture"/>);</c>
		/// </remarks>
		/// <param name="provider">Defaults to <see cref="InvariantCulture"/>.</param>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string ToText(IFormatProvider? provider = null)
			=> @this.ToString("c", provider ?? CultureInfo.InvariantCulture);
	}
}
