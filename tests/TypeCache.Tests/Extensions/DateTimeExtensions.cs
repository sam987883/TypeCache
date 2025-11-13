// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class DateTimeExtensions
{
	[Fact]
	public void As()
	{
		var now = DateTime.Now;
		var utcNow = DateTime.UtcNow;

		Assert.Equal(DateTimeKind.Local, utcNow.As(DateTimeKind.Local).Kind);
		Assert.Equal(DateTimeKind.Unspecified, utcNow.As(DateTimeKind.Unspecified).Kind);
		Assert.Equal(DateTimeKind.Utc, now.As(DateTimeKind.Utc).Kind);
	}

	[Fact]
	public void ChangeTimeZone()
	{
		var now = DateTime.Now;

		Assert.Equal(now, now.ChangeTimeZone(TimeZoneInfo.Local, TimeZoneInfo.Utc).ChangeTimeZone(TimeZoneInfo.Utc, TimeZoneInfo.Local));

		Assert.Equal(now, now.ChangeTimeZone(TimeZoneInfo.Local.Id, TimeZoneInfo.Utc.Id).ChangeTimeZone(TimeZoneInfo.Utc.Id, TimeZoneInfo.Local.Id));
	}

	[Fact]
	public void ConvertTimeToUTC()
	{
		var now = DateTime.Now;

		Assert.Equal(now, now.ToUTC(TimeZoneInfo.Local).ChangeTimeZone(TimeZoneInfo.Utc.Id, TimeZoneInfo.Local.Id));
	}

	[Fact]
	public void DateOnly()
	{
		var now = DateTime.UtcNow;
		var nowOffset = DateTimeOffset.Now;

		Assert.Equal(System.DateOnly.FromDateTime(now), now.DateOnly);
		Assert.Equal(System.DateOnly.FromDateTime(nowOffset.DateTime), nowOffset.DateOnly);
	}

	[Fact]
	public void TimeOnly()
	{
		var now = DateTime.UtcNow;
		var nowOffset = DateTimeOffset.Now;

		Assert.Equal(System.TimeOnly.FromDateTime(now), now.TimeOnly);
		Assert.Equal(System.TimeOnly.FromDateTime(nowOffset.DateTime), nowOffset.TimeOnly);
	}

	[Fact]
	public void ToDateTimeOffset()
	{
		var now = DateTime.Now;
		var utcNow = DateTime.UtcNow;
		var localOffset = new DateTimeOffset(now).Offset;
		var utcOffset = new DateTimeOffset(utcNow).Offset;

		Assert.Equal(new DateTimeOffset(now), now.ToDateTimeOffset());
		Assert.Equal(new DateTimeOffset(now, localOffset), now.ToDateTimeOffset(localOffset));
		Assert.Equal(new DateTimeOffset(utcNow), utcNow.ToDateTimeOffset());
		Assert.Equal(new DateTimeOffset(utcNow, utcOffset), utcNow.ToDateTimeOffset(utcOffset));
	}

	[Fact]
	public void ToISO8601()
	{
		Assert.Equal(System.DateOnly.MaxValue.ToString("yyyy-MM-dd"), System.DateOnly.MaxValue.ToISO8601());
		Assert.Equal(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), DateTime.Now.ToISO8601());
		Assert.Equal(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"), DateTime.UtcNow.ToISO8601());
		Assert.Equal(DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ssK"), DateTimeOffset.Now.ToISO8601());
		Assert.Equal(DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssK"), DateTimeOffset.UtcNow.ToISO8601());
		Assert.Equal(System.TimeOnly.MaxValue.ToString("HH:mm:ss"), System.TimeOnly.MaxValue.ToISO8601());
	}

	[Fact]
	public void ToText()
	{
		Assert.Equal(TimeSpan.MaxValue.ToString(), TimeSpan.MaxValue.ToText());
	}

	[Fact]
	public void ToTimeZone()
	{
		var now = DateTime.Now;
		var nowOffset = DateTimeOffset.Now;
		var utcNow = DateTime.UtcNow;

		Assert.Equal(now, now.ToTimeZone(DateTimeKind.Utc).ToTimeZone(DateTimeKind.Local));
		Assert.Equal(utcNow, utcNow.ToTimeZone(DateTimeKind.Local).ToTimeZone(DateTimeKind.Utc));
		Assert.Equal(now, now.ToTimeZone(TimeZoneInfo.Utc).ToTimeZone(TimeZoneInfo.Local));
		Assert.Equal(nowOffset, nowOffset.ToTimeZone(TimeZoneInfo.Utc).ToTimeZone(TimeZoneInfo.Local));
		Assert.Equal(nowOffset, nowOffset.ToTimeZone(TimeZoneInfo.Utc.Id).ToTimeZone(TimeZoneInfo.Local.Id));
	}

	[Fact]
	public void ToUTC()
	{
		var now = DateTime.Now;

		Assert.Equal(TimeZoneInfo.ConvertTimeToUtc(now), now.ToUTC());
		Assert.Equal(TimeZoneInfo.ConvertTimeToUtc(now, TimeZoneInfo.Local), now.ToUTC(TimeZoneInfo.Local));
	}
}
