// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions
{
	public static class DateTimeExtensions
	{
		/// <summary>
		/// <see cref="DateTime.SpecifyKind(DateTime, DateTimeKind)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime As(this DateTime @this, DateTimeKind kind)
			=> DateTime.SpecifyKind(@this, kind);

		/// <summary>
		/// <see cref="TimeZoneInfo.ConvertTime(DateTime, TimeZoneInfo, TimeZoneInfo)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime ConvertTime(this DateTime @this, TimeZoneInfo sourceTimeZone, TimeZoneInfo targetTimeZone)
			=> TimeZoneInfo.ConvertTime(@this, sourceTimeZone, targetTimeZone);

		/// <summary>
		/// <see cref="TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime, string, string)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime ConvertTime(this DateTime @this, string sourceSystemTimeZoneId, string targetSystemTimeZoneId)
			=> TimeZoneInfo.ConvertTimeBySystemTimeZoneId(@this, sourceSystemTimeZoneId, targetSystemTimeZoneId);

		/// <summary>
		/// <see cref="TimeZoneInfo.ConvertTimeToUtc(DateTime, TimeZoneInfo)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime ConvertTimeToUTC(this DateTime @this, TimeZoneInfo sourceTimeZone)
			=> TimeZoneInfo.ConvertTimeToUtc(@this, sourceTimeZone);

		/// <summary>
		/// <code>
		/// <list type="table">
		/// <item><term>kind == @this.Kind</term> <description>@this</description></item>
		/// <item><term><see cref="DateTimeKind.Unspecified"/></term> <description>@this.As(kind)</description></item>
		/// <item><term><see cref="DateTimeKind.Local"/></term> <description><see cref="TimeZoneInfo.ConvertTimeToUtc(DateTime)"/></description></item>
		/// <item><term><see cref="DateTimeKind.Utc"/></term> <description><see cref="TimeZoneInfo.ConvertTimeFromUtc"/>(<see cref="DateTime"/>, <see cref="TimeZoneInfo"/>.Local)</description></item>
		/// </list>
		/// </code>
		/// </summary>
		public static DateTime To(this DateTime @this, DateTimeKind kind)
			=> kind switch
			{
				_ when kind == @this.Kind => @this,
				DateTimeKind.Unspecified => @this.As(kind),
				DateTimeKind.Local => TimeZoneInfo.ConvertTimeToUtc(@this),
				DateTimeKind.Utc => TimeZoneInfo.ConvertTimeFromUtc(@this, TimeZoneInfo.Local),
				_ => throw new ArgumentOutOfRangeException($"{nameof(To)}: {nameof(DateTimeKind)} value of {kind} is not supported.")
			};

		/// <summary>
		/// <code>
		/// <list type="table">
		/// <item><term><see cref="DateTimeKind.Local"/></term> <description><see cref="TimeZoneInfo.ConvertTime(DateTime, TimeZoneInfo)"/></description></item>
		/// <item><term><see cref="DateTimeKind.Utc"/></term> <description><see cref="TimeZoneInfo.ConvertTimeFromUtc(DateTime, TimeZoneInfo)"/></description></item>
		/// </list>
		/// </code>
		/// </summary>
		public static DateTime To(this DateTime @this, TimeZoneInfo targetTimeZone)
			=> @this.Kind switch
			{
				DateTimeKind.Local => TimeZoneInfo.ConvertTime(@this, targetTimeZone),
				DateTimeKind.Utc => TimeZoneInfo.ConvertTimeFromUtc(@this, targetTimeZone),
				_ => throw new TimeZoneNotFoundException($"{nameof(ConvertTime)}: {nameof(DateTime)} value must have a {nameof(DateTimeKind)} of {DateTimeKind.Local} or {DateTimeKind.Utc}.")
			};

		/// <summary>
		/// <see cref="TimeZoneInfo.ConvertTime(DateTimeOffset, TimeZoneInfo)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTimeOffset To(this DateTimeOffset @this, TimeZoneInfo targetTimeZone)
			=> TimeZoneInfo.ConvertTime(@this, targetTimeZone);

		/// <summary>
		/// <see cref="TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTimeOffset, string)"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTimeOffset To(this DateTimeOffset @this, string targetSystemTimeZoneId)
			=> TimeZoneInfo.ConvertTimeBySystemTimeZoneId(@this, targetSystemTimeZoneId);
	}
}
