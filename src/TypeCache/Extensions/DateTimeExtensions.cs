// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions
{
	public static class DateTimeExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime As(this DateTime @this, DateTimeKind kind)
			=> DateTime.SpecifyKind(@this, kind);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime ConvertTime(this DateTime @this, TimeZoneInfo sourceTimeZone, TimeZoneInfo targetTimeZone)
			=> TimeZoneInfo.ConvertTime(@this, sourceTimeZone, targetTimeZone);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime ConvertTime(this DateTime @this, string sourceSystemTimeZoneId, string targetSystemTimeZoneId)
			=> TimeZoneInfo.ConvertTimeBySystemTimeZoneId(@this, sourceSystemTimeZoneId, targetSystemTimeZoneId);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime ConvertTimeToUTC(this DateTime @this, TimeZoneInfo sourceTimeZone)
			=> TimeZoneInfo.ConvertTimeToUtc(@this, sourceTimeZone);

		public static DateTime To(this DateTime @this, DateTimeKind kind)
			=> kind switch
			{
				_ when kind == @this.Kind => @this,
				DateTimeKind.Unspecified => @this.As(kind),
				DateTimeKind.Local => TimeZoneInfo.ConvertTimeToUtc(@this),
				DateTimeKind.Utc => TimeZoneInfo.ConvertTimeFromUtc(@this, TimeZoneInfo.Local),
				_ => throw new ArgumentOutOfRangeException($"{nameof(To)}: {nameof(DateTimeKind)} value of {kind} is not supported.")
			};

		public static DateTime To(this DateTime @this, TimeZoneInfo targetTimeZone)
			=> @this.Kind switch
			{
				DateTimeKind.Local => TimeZoneInfo.ConvertTime(@this, targetTimeZone),
				DateTimeKind.Utc => TimeZoneInfo.ConvertTimeFromUtc(@this, targetTimeZone),
				_ => throw new TimeZoneNotFoundException($"{nameof(ConvertTime)}: {nameof(DateTime)} value must have a {nameof(DateTimeKind)} of {DateTimeKind.Local} or {DateTimeKind.Utc}.")
			};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTimeOffset To(this DateTimeOffset @this, TimeZoneInfo targetTimeZone)
			=> TimeZoneInfo.ConvertTime(@this, targetTimeZone);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTimeOffset To(this DateTimeOffset @this, string targetSystemTimeZoneId)
			=> TimeZoneInfo.ConvertTimeBySystemTimeZoneId(@this, targetSystemTimeZoneId);
	}
}
