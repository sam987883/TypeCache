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
	public void ToDateOnly()
	{
		var now = DateTime.UtcNow;
		var nowOffset = DateTimeOffset.Now;

		Assert.Equal(DateOnly.FromDateTime(now), now.ToDateOnly());
		Assert.Equal(DateOnly.FromDateTime(nowOffset.DateTime), nowOffset.ToDateOnly());
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
	public void ToTimeOnly()
	{
		var now = DateTime.UtcNow;
		var nowOffset = DateTimeOffset.Now;

		Assert.Equal(TimeOnly.FromDateTime(now), now.ToTimeOnly());
		Assert.Equal(TimeOnly.FromDateTime(nowOffset.DateTime), nowOffset.ToTimeOnly());
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
