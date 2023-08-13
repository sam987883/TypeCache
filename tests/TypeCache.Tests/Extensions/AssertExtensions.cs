// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Collections;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class AssertExtensions
{
	[Fact]
	public void AssertEquals()
	{
		123456.AssertEquals(123456);
		"AAA".AssertEquals("AAA");
		(null as string).AssertEquals(null);
		"AAA".AssertEquals("AAA", StringComparer.Ordinal);
		"AAA".AssertEquals("aaa", StringComparison.OrdinalIgnoreCase);
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => 123.AssertEquals(456));
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".AssertEquals("bbb"));
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => (null as string).AssertEquals("bbb"));
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".AssertEquals("bbb", StringComparer.Ordinal));
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".AssertEquals(null, null));
	}

	[Fact]
	public void AssertFalse()
	{
		false.AssertFalse();
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => true.AssertFalse());
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
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => Array<int>.Empty.AssertNotEmpty());
	}

	[Fact]
	public void AssertNotEquals()
	{
		123456.AssertNotEquals(12345);
		"AAA".AssertNotEquals("AA");
		(null as string).AssertNotEquals(string.Empty);
		"AAA".AssertNotEquals("BBB", StringComparer.Ordinal);
		"AAA".AssertNotEquals("bbb", StringComparison.OrdinalIgnoreCase);
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => 123.AssertNotEquals(123));
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => "bbb".AssertNotEquals("bbb"));
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => (null as string).AssertNotEquals(null as string));
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".AssertNotEquals("AAA", StringComparer.Ordinal));
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".AssertNotEquals("AAA", null));
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

	[Fact]
	public void AssertTrue()
	{
		true.AssertTrue();
		Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => false.AssertTrue());
	}
}
