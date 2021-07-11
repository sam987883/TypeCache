// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions
{
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
		public void ConvertTime()
		{
			var now = DateTime.Now;

			Assert.Equal(now, now.ConvertTime(TimeZoneInfo.Local, TimeZoneInfo.Utc).ConvertTime(TimeZoneInfo.Utc, TimeZoneInfo.Local));

			Assert.Equal(now, now.ConvertTime(TimeZoneInfo.Local.Id, TimeZoneInfo.Utc.Id).ConvertTime(TimeZoneInfo.Utc.Id, TimeZoneInfo.Local.Id));
		}

		[Fact]
		public void ConvertTimeToUTC()
		{
			var now = DateTime.Now;

			Assert.Equal(now, now.ConvertTimeToUTC(TimeZoneInfo.Local).ConvertTime(TimeZoneInfo.Utc.Id, TimeZoneInfo.Local.Id));
		}

		[Fact]
		public void To()
		{
			var now = DateTime.Now;
			var nowOffset = DateTimeOffset.Now;
			var utcNow = DateTime.UtcNow;

			Assert.Equal(now, now.To(DateTimeKind.Utc).To(DateTimeKind.Local));
			Assert.Equal(utcNow, utcNow.To(DateTimeKind.Local).To(DateTimeKind.Utc));

			Assert.Equal(now, now.To(TimeZoneInfo.Utc).To(TimeZoneInfo.Local));
			Assert.Equal(nowOffset, nowOffset.To(TimeZoneInfo.Utc).To(TimeZoneInfo.Local));

			Assert.Equal(nowOffset, nowOffset.To(TimeZoneInfo.Utc.Id).To(TimeZoneInfo.Local.Id));
		}
	}
}
