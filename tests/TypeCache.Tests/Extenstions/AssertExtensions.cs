// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Collections;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class AssertExtensions
{
	[Fact]
	public void Assert()
	{
		123456.Assert(123456);
		"AAA".Assert("AAA");
		(null as string).Assert(null);
		"AAA".Assert("AAA", StringComparer.Ordinal);
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => 123.Assert(456));
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".Assert("bbb"));
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => (null as string).Assert("bbb"));
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".Assert("bbb", StringComparer.Ordinal));
		Xunit.Assert.Throws<ArgumentNullException>(() => "AAA".Assert(null, null));
	}

	[Fact]
	public void AssertNotBlank()
	{
		"AAA".AssertNotBlank();
		Xunit.Assert.Throws<ArgumentNullException>(() => (null as string).AssertNotBlank());
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => string.Empty.AssertNotBlank());
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => "      ".AssertNotBlank());
	}

	[Fact]
	public void AssertNotEmpty()
	{
		"AAA".AssertNotEmpty();
		Xunit.Assert.Throws<ArgumentNullException>(() => (null as string).AssertNotEmpty());
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => string.Empty.AssertNotEmpty());
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => Enumerable<int>.Empty.AssertNotEmpty());
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => Array<int>.Empty.AssertNotEmpty());
	}

	[Fact]
	public void AssertNotNull()
	{
		((int?)123456).AssertNotNull();
		"AAA".AssertNotNull();
		Xunit.Assert.Throws<ArgumentNullException>(() => (null as string).AssertNotNull());
		Xunit.Assert.Throws<ArgumentNullException>(() => (null as int?).AssertNotNull());
	}

	[Fact]
	public void AssertNotSame()
	{
		var a = new object();
		var b = new object();

		(a, b).AssertNotSame();
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => (a, a).AssertNotSame());
	}
}
