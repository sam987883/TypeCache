// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;
using TypeCache.Utilities;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class ThrowIfExtensions
{
	[Fact]
	public void ThrowIfNotEqual()
	{
		123456.ThrowIfNotEqual(123456);
		"AAA".ThrowIfNotEqual("AAA");
		(null as string).ThrowIfNotEqual(null);
		"AAA".ThrowIfNotEqual("AAA", StringComparer.Ordinal);
		Assert.Throws<ArgumentOutOfRangeException>(() => 123.ThrowIfNotEqual(456));
		Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".ThrowIfNotEqual("bbb"));
		Assert.Throws<ArgumentOutOfRangeException>(() => (null as string).ThrowIfNotEqual("bbb"));
		Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".ThrowIfNotEqual("bbb", StringComparer.Ordinal));
		Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".ThrowIfNotEqual(null, null));
	}

	[Fact]
	public void ThrowIfNotEqualIgnoreCase()
	{
		"AAA".ThrowIfNotEqualIgnoreCase("aaa");
		"AAA".ThrowIfNotEqualIgnoreCase("AAA");
		Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".ThrowIfNotEqualIgnoreCase("bbb"));
	}

	[Fact]
	public void ThrowIfTrue()
	{
		false.ThrowIfTrue();
		Assert.Throws<ArgumentOutOfRangeException>(() => true.ThrowIfTrue());
	}

	[Fact]
	public void ThrowIfBlank()
	{
		"AAA".ThrowIfBlank();
		Assert.Throws<ArgumentOutOfRangeException>(() => (null as string).ThrowIfBlank());
		Assert.Throws<ArgumentOutOfRangeException>(() => string.Empty.ThrowIfBlank());
		Assert.Throws<ArgumentOutOfRangeException>(() => "      ".ThrowIfBlank());
	}

	[Fact]
	public void ThrowIfEmpty()
	{
		"AAA".ThrowIfEmpty();
		Assert.Throws<ArgumentNullException>(() => (null as string).ThrowIfEmpty());
		Assert.Throws<ArgumentOutOfRangeException>(() => string.Empty.ThrowIfEmpty());
		Assert.Throws<ArgumentOutOfRangeException>(() => Array<int>.Empty.ThrowIfEmpty());
	}

	[Fact]
	public void ThrowIfEqual()
	{
		123456.ThrowIfEqual(12345);
		"AAA".ThrowIfEqual("AA");
		(null as string).ThrowIfEqual(string.Empty);
		"AAA".ThrowIfEqual("BBB", StringComparer.Ordinal);
		Assert.Throws<ArgumentOutOfRangeException>(() => 123.ThrowIfEqual(123));
		Assert.Throws<ArgumentOutOfRangeException>(() => "bbb".ThrowIfEqual("bbb"));
		Assert.Throws<ArgumentOutOfRangeException>(() => (null as string).ThrowIfEqual(null as string));
		Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".ThrowIfEqual("AAA", StringComparer.Ordinal));
		Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".ThrowIfEqual("AAA", null));
	}

	[Fact]
	public void ThrowIfEqualIgnoreCase()
	{
		"AAA".ThrowIfEqualIgnoreCase("bbb");
		"AAA".ThrowIfEqualIgnoreCase("aba");
		Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".ThrowIfEqualIgnoreCase("aaa"));
		Assert.Throws<ArgumentOutOfRangeException>(() => "ABA".ThrowIfEqualIgnoreCase("aba"));
	}

	[Fact]
	public void ThrowIfNull()
	{
		((int?)123456).ThrowIfNull();
		"AAA".ThrowIfNull();
		Assert.Throws<ArgumentNullException>(() => (null as string).ThrowIfNull());
		Assert.Throws<ArgumentNullException>(() => (null as int?).ThrowIfNull());
	}

	[Fact]
	public void ThrowIfSame()
	{
		var a = new object();
		var b = new object();

		(a, b).ThrowIfSame();
		Assert.Throws<ArgumentOutOfRangeException>(() => (a, a).ThrowIfSame());
	}

	[Fact]
	public void ThrowIfFalse()
	{
		true.ThrowIfFalse();
		Assert.Throws<ArgumentOutOfRangeException>(() => false.ThrowIfFalse());
	}
}
