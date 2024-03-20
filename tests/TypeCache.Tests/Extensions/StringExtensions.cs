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
	public void Has()
	{
		Assert.True(TEST_STRING.ContainsIgnoreCase("BCC 1"));
	}

	[Fact]
	public void Is()
	{
		Assert.True(TEST_STRING.EqualsIgnoreCase("AABBCC 123 `~!#$%^\t\r\n"));
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
	public void Left()
	{
		Assert.Equal("AaBbCc 1", TEST_STRING.Left(8));
		Assert.Equal(string.Empty, TEST_STRING.Left(0));

		Assert.True(TEST_STRING.StartsWithIgnoreCase("AABBCC 123"));
	}

	[Fact]
	public void Mask()
	{
		Assert.Equal(string.Empty, (null as string).Mask());
		Assert.Equal("++++++ +++ `~!#$%^\t\r\n", TEST_STRING.Mask('+'));
	}

	[Fact]
	public void MaskHide()
	{
		Assert.Equal("--Bb-- 123 `~!#$%^\t\r\n", TEST_STRING.MaskHide('-', StringComparison.OrdinalIgnoreCase, ["A", "C", "\t\r\n"]));
	}

	[Fact]
	public void MaskShow()
	{
		Assert.Equal("ooBboo 123 `~!#$%^\t\r\n", TEST_STRING.MaskShow('o', StringComparison.Ordinal, ["Bb", " ", "123", "`~!#$%^"]));
	}

	[Fact]
	public void Reverse()
	{
		Assert.Equal(string.Empty, string.Empty.Reverse());
		Assert.Equal("321 cCbBaA", "AaBbCc 123".Reverse());
	}

	[Fact]
	public void Right()
	{
		Assert.True("321 cCbBaA".EndsWithIgnoreCase("ccbbaa"));
	}

	[Fact]
	public void ToEnum()
	{
		Assert.Equal(StringComparison.Ordinal, nameof(StringComparison.Ordinal).ToEnum<StringComparison>());
		Assert.Equal(StringComparison.OrdinalIgnoreCase, nameof(StringComparison.OrdinalIgnoreCase).ToUpperInvariant().ToEnum<StringComparison>());
	}

	[Fact]
	public void ToStringSegment()
	{
		Assert.Equal(new StringSegment(TEST_STRING), TEST_STRING.ToStringSegment());
		Assert.Equal(new StringSegment(TEST_STRING, 2, 0), TEST_STRING.ToStringSegment(2, 0));
		Assert.Equal(new StringSegment(TEST_STRING, 2, 3), TEST_STRING.ToStringSegment(2, 3));
		Assert.Equal(new StringSegment(TEST_STRING, 9, 1), TEST_STRING.ToStringSegment(9, 1));
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
