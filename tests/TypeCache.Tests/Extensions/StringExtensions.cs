// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Primitives;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class StringExtensions
{
	private const string TEST_STRING = "AaBbCc 123 `~!#$%^\t\r\n";

	[Fact]
	public void ContainsIgnoreCase()
	{
		Assert.True(TEST_STRING.ContainsIgnoreCase("BCC 1"));
	}

	[Fact]
	public void EndsWithIgnoreCase()
	{
		Assert.True("321 cCbBaA".EndsWithIgnoreCase("ccbbaa"));
	}

	[Fact]
	public void Enum()
	{
		Assert.Equal(StringComparison.Ordinal, nameof(StringComparison.Ordinal).Enum<StringComparison>());
		Assert.Equal(StringComparison.OrdinalIgnoreCase, nameof(StringComparison.OrdinalIgnoreCase).ToUpperInvariant().Enum<StringComparison>());
	}

	[Fact]
	public void EqualsIgnoreCase()
	{
		Assert.True(TEST_STRING.EqualsIgnoreCase("AABBCC 123 `~!#$%^\t\r\n"));
	}

	[Fact]
	public void FromBase64()
	{
		Assert.Equal(TEST_STRING, TEST_STRING.ToBase64(Encoding.ASCII).FromBase64(Encoding.ASCII));
		Assert.Equal(TEST_STRING, TEST_STRING.ToBase64(Encoding.ASCII).FromBase64(Encoding.UTF8));
	}

	[Fact]
	public void ToBase64()
	{
		Assert.Equal(TEST_STRING, TEST_STRING.ToBase64(Encoding.ASCII).FromBase64(Encoding.ASCII));
		Assert.Equal(TEST_STRING, TEST_STRING.ToBase64(Encoding.ASCII).FromBase64(Encoding.UTF8));
	}

	[Fact]
	public void IsBlank()
	{
		Assert.True(string.Empty.IsBlank());
		Assert.True(" \t \r \n ".IsBlank());
		Assert.True((null as string).IsBlank());
		Assert.False(TEST_STRING.IsBlank());
	}

	[Fact]
	public void IsNotBlank()
	{
		Assert.False(string.Empty.IsNotBlank());
		Assert.False(" \t \r \n ".IsNotBlank());
		Assert.False((null as string).IsNotBlank());
		Assert.True(TEST_STRING.IsNotBlank());
	}

	[Fact]
	public void Join()
	{
		Assert.Equal(TEST_STRING, " ".Join(["AaBbCc", "123", "`~!#$%^\t\r\n"]));
		Assert.Equal(TEST_STRING, " ".Join([TEST_STRING]));
	}

	[Fact]
	public void Mask()
	{
		Assert.Equal(string.Empty, (null as string).Mask());
		Assert.Equal("++++++ +++ `~!#$%^\t\r\n", TEST_STRING.Mask('+'));
		Assert.Equal("+aB+++ 123 `~!#$%^\t\r\n", TEST_STRING.Mask('+', ["A", "b", "Cc"]));
	}

	[Fact]
	public void MaskIgnoreCase()
	{
		Assert.Equal("++++++ +++ `~!#$%^\t\r\n", TEST_STRING.MaskIgnoreCase('+'));
		Assert.Equal("++++++ 123 `~!#$%^\t\r\n", TEST_STRING.MaskIgnoreCase('+', ["A", "b", "C"]));
	}

	[Fact]
	public void Reverse()
	{
		Assert.Equal(string.Empty, string.Empty.Reverse());
		Assert.Equal("321 cCbBaA", "AaBbCc 123".Reverse());
	}

	[Fact]
	public void StringSegment()
	{
		Assert.Equal(new StringSegment(TEST_STRING), TEST_STRING.StringSegment());
		Assert.Equal(new StringSegment(TEST_STRING, 2, 0), TEST_STRING.StringSegment(2, 0));
		Assert.Equal(new StringSegment(TEST_STRING, 2, 3), TEST_STRING.StringSegment(2, 3));
		Assert.Equal(new StringSegment(TEST_STRING, 9, 1), TEST_STRING.StringSegment(9, 1));
	}

	[Fact]
	public void StartsWithIgnoreCase()
	{
		Assert.Equal("AaBbCc 1", TEST_STRING.Left(8));
		Assert.Equal(string.Empty, TEST_STRING.Left(0));

		Assert.True(TEST_STRING.StartsWithIgnoreCase("AABBCC 123"));
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
	public void TrimEnd()
	{
		Assert.Equal("AaB", TEST_STRING.TrimEnd("BCC 123 `~!#$%^\t\r\n"));
		Assert.NotEqual("AaB", TEST_STRING.TrimEnd("BCC 123 `~!#$%^\t\r\n", StringComparison.Ordinal));
		Assert.Equal(TEST_STRING, TEST_STRING.TrimEnd("******"));
	}

	[Fact]
	public void TrimStart()
	{
		Assert.Equal(" `~!#$%^\t\r\n", TEST_STRING.TrimStart("aabbcc 123"));
		Assert.NotEqual(" `~!#$%^\t\r\n", TEST_STRING.TrimStart("aabbcc 123", StringComparison.Ordinal));
		Assert.Equal(TEST_STRING, TEST_STRING.TrimStart("******"));
	}
}
