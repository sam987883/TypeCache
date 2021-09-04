// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions
{
	public class AssertExtensions
	{
		[Fact]
		public void Assert()
		{
			const string NAME = "TestName";

			123456.Assert(NAME, 123456);
			"AAA".Assert(NAME, "AAA");
			(null as string).Assert(NAME, null);
			"AAA".Assert(NAME, "AAA", StringComparer.Ordinal);
			Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => 123.Assert(NAME, 456));
			Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".Assert(NAME, "bbb"));
			Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => (null as string).Assert(NAME, "bbb"));
			Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".Assert(NAME, "bbb", StringComparer.Ordinal));
			Xunit.Assert.Throws<ArgumentNullException>(() => "AAA".Assert(NAME, null, null));
		}

		[Fact]
		public void AssertNotNull()
		{
			const string NAME = nameof(AssertNotNull);

			((int?)123456).AssertNotNull(NAME);
			"AAA".AssertNotNull(NAME);
			Xunit.Assert.Throws<ArgumentNullException>(() => (null as string).AssertNotNull(NAME));
			Xunit.Assert.Throws<ArgumentNullException>(() => (null as int?).AssertNotNull(NAME));
		}

		[Fact]
		public void AssertNotBlank()
		{
			const string NAME = nameof(AssertNotBlank);

			"AAA".AssertNotBlank(NAME);
			Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => (null as string).AssertNotBlank(NAME));
			Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => string.Empty.AssertNotBlank(NAME));
			Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => "      ".AssertNotBlank(NAME));
		}

		[Fact]
		public void AssertNotSame()
		{
			var a = new object();
			var b = new object();

			(a, b).AssertNotSame((nameof(a), nameof(b)));
			Xunit.Assert.Throws<ArgumentException>(() => (a, a).AssertNotSame((nameof(a), nameof(a))));
		}
	}
}
