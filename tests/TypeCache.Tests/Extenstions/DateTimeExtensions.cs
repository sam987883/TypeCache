// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Collections.Extensions;
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

		Assert.Equal(now, now.ConvertTimeToUTC(TimeZoneInfo.Local).ChangeTimeZone(TimeZoneInfo.Utc.Id, TimeZoneInfo.Local.Id));
	}

	[Fact]
	public void To()
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
}
