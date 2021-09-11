// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using static TypeCache.Default;

namespace TypeCache.Extensions
{
	public static class DateTimeExtensions
	{
		/// <summary>
		/// <c><see cref="DateTime.SpecifyKind(DateTime, DateTimeKind)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static DateTime As(this DateTime @this, DateTimeKind kind)
			=> DateTime.SpecifyKind(@this, kind);

		/// <summary>
		/// <c><see cref="TimeZoneInfo.ConvertTime(DateTime, TimeZoneInfo, TimeZoneInfo)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static DateTime ConvertTime(this DateTime @this, TimeZoneInfo sourceTimeZone, TimeZoneInfo targetTimeZone)
			=> TimeZoneInfo.ConvertTime(@this, sourceTimeZone, targetTimeZone);

		/// <summary>
		/// <c><see cref="TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime, string, string)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static DateTime ConvertTime(this DateTime @this, string sourceSystemTimeZoneId, string targetSystemTimeZoneId)
			=> TimeZoneInfo.ConvertTimeBySystemTimeZoneId(@this, sourceSystemTimeZoneId, targetSystemTimeZoneId);

		/// <summary>
		/// <c><see cref="TimeZoneInfo.ConvertTimeToUtc(DateTime, TimeZoneInfo)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static DateTime ConvertTimeToUTC(this DateTime @this, TimeZoneInfo sourceTimeZone)
			=> TimeZoneInfo.ConvertTimeToUtc(@this, sourceTimeZone);

		/// <summary>
		/// <list type="table">
		/// <item><c><term><paramref name="kind"/> == @<paramref name="this"/>.Kind</term> @<paramref name="this"/></c></item>
		/// <item><c><term><see cref="DateTimeKind.Local"/></term> <see cref="TimeZoneInfo.ConvertTimeFromUtc"/>(<see cref="DateTime"/>, <see cref="TimeZoneInfo"/>.Local)</c></item>
		/// <item><c><term><see cref="DateTimeKind.Unspecified"/></term> @<paramref name="this"/>.As(kind)</c></item>
		/// <item><c><term><see cref="DateTimeKind.Utc"/></term> <see cref="TimeZoneInfo.ConvertTimeToUtc(DateTime)"/></c></item>
		/// </list>
		/// </summary>
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
		/// <list type="table">
		/// <item><c><term><see cref="DateTimeKind.Local"/></term> <see cref="TimeZoneInfo.ConvertTime(DateTime, TimeZoneInfo)"/></c></item>
		/// <item><c><term><see cref="DateTimeKind.Utc"/></term> <see cref="TimeZoneInfo.ConvertTimeFromUtc(DateTime, TimeZoneInfo)"/></c></item>
		/// </list>
		/// </summary>
		public static DateTime To(this DateTime @this, TimeZoneInfo targetTimeZone)
			=> @this.Kind switch
			{
				DateTimeKind.Local => TimeZoneInfo.ConvertTime(@this, targetTimeZone),
				DateTimeKind.Utc => TimeZoneInfo.ConvertTimeFromUtc(@this, targetTimeZone),
				_ => throw new TimeZoneNotFoundException($"{nameof(ConvertTime)}: {nameof(DateTime)} value must have a {nameof(DateTimeKind)} of {DateTimeKind.Local} or {DateTimeKind.Utc}.")
			};

		/// <summary>
		/// <c><see cref="TimeZoneInfo.ConvertTime(DateTimeOffset, TimeZoneInfo)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static DateTimeOffset To(this DateTimeOffset @this, TimeZoneInfo targetTimeZone)
			=> TimeZoneInfo.ConvertTime(@this, targetTimeZone);

		/// <summary>
		/// <c><see cref="TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTimeOffset, string)"/></c>
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static DateTimeOffset To(this DateTimeOffset @this, string targetSystemTimeZoneId)
			=> TimeZoneInfo.ConvertTimeBySystemTimeZoneId(@this, targetSystemTimeZoneId);
	}
}
