// Copyright (c) 2021 Samuel Abraham

using System;
using System.Text;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class StringExtensions
{
	[Fact]
	public void FromBase64()
	{
		const string TEST_STRING = "AaBbCc 123 `~!#$%^\t\r\n";

		Assert.Equal(TEST_STRING, TEST_STRING.ToBase64(Encoding.ASCII).FromBase64(Encoding.ASCII));
		Assert.Equal(TEST_STRING, TEST_STRING.ToBase64(Encoding.ASCII).FromBase64(Encoding.UTF8));
	}

	[Fact]
	public void ToBase64()
	{
		const string TEST_STRING = "AaBbCc 123 `~!#$%^\t\r\n";

		Assert.Equal(TEST_STRING, TEST_STRING.ToBase64(Encoding.ASCII).FromBase64(Encoding.ASCII));
		Assert.Equal(TEST_STRING, TEST_STRING.ToBase64(Encoding.ASCII).FromBase64(Encoding.UTF8));
	}

	[Fact]
	public void Has()
	{
		const string TEST_STRING = "AaBbCc 123 `~!#$%^\t\r\n";

		Assert.True(TEST_STRING.Has("BCC 1"));
		Assert.False(TEST_STRING.Has("BCC 1", StringComparison.Ordinal));
	}

	[Fact]
	public void Is()
	{
		const string TEST_STRING = "AaBbCc 123 `~!#$%^\t\r\n";

		Assert.True(TEST_STRING.Is("AABBCC 123 `~!#$%^\t\r\n"));
		Assert.False(TEST_STRING.Is("AABBCC 123 `~!#$%^\t\r\n", StringComparison.Ordinal));
	}

	[Fact]
	public void IsBlank()
	{
		const string TEST_STRING = "AaBbCc 123 `~!#$%^\t\r\n";

		Assert.True(string.Empty.IsBlank());
		Assert.True(" \t \r \n ".IsBlank());
		Assert.True((null as string).IsBlank());
		Assert.False(TEST_STRING.IsBlank());
	}

	[Fact]
	public void Join()
	{
		const string TEST_STRING = "AaBbCc 123 `~!#$%^\t\r\n";

		Assert.Equal(TEST_STRING, " ".Join("AaBbCc", "123", "`~!#$%^\t\r\n"));
		Assert.Equal(TEST_STRING, " ".Join(TEST_STRING));
	}

	[Fact]
	public void Left()
	{
		const string TEST_STRING = "AaBbCc 123 `~!#$%^\t\r\n";

		Assert.Equal("AaBbCc 1", TEST_STRING.Left(8));
		Assert.Equal(string.Empty, TEST_STRING.Left(0));

		Assert.True(TEST_STRING.Left('A'));
		Assert.False(TEST_STRING.Left('a'));

		Assert.True(TEST_STRING.Left("AABBCC 123"));
		Assert.False(TEST_STRING.Left("AABBCC 123", StringComparison.Ordinal));
	}

	[Fact]
	public void Mask()
	{
		const string TEST_STRING = "AaBbCc 123 `~!#$%^\t\r\n";

		Assert.Equal(string.Empty, (null as string).Mask());
		Assert.Equal("++++++ +++ `~!#$%^\t\r\n", TEST_STRING.Mask('+'));
	}

	[Fact]
	public void MaskHide()
	{
		const string TEST_STRING = "AaBbCc 123 `~!#$%^\t\r\n";

		Assert.Equal("--Bb-- 123 `~!#$%^\t\r\n", TEST_STRING.MaskHide('-', StringComparison.OrdinalIgnoreCase, "A", "C", "\t\r\n"));
	}

	[Fact]
	public void MaskShow()
	{
		const string TEST_STRING = "AaBbCc 123 `~!#$%^\t\r\n";

		Assert.Equal("ooBboo 123 `~!#$%^\t\r\n", TEST_STRING.MaskShow('o', StringComparison.Ordinal, "Bb", " ", "123", "`~!#$%^"));
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
		Assert.True("321 cCbBaA".Right('A'));
		Assert.False("321 cCbBaA".Right('a'));

		Assert.True("321 cCbBaA".Right("ccbbaa"));
		Assert.False("321 cCbBaA".Right("ccbbaa", StringComparison.Ordinal));
	}

	[Fact]
	public void ToBytes()
	{
		const string TEST_STRING = "AaBbCc 123 `~!#$%^\t\r\n";

		Assert.Equal(TEST_STRING, TEST_STRING.ToBytes(Encoding.UTF8).ToText(Encoding.UTF8));

		Assert.Equal("BbCc 1", TEST_STRING.ToBytes(Encoding.UTF8).ToText(Encoding.UTF8, 2, 6));

		Assert.Equal(TEST_STRING, ((ReadOnlySpan<byte>)TEST_STRING.ToBytes(Encoding.UTF8).AsSpan()).ToText(Encoding.UTF8));
	}

	[Fact]
	public void ToText()
	{
		const string TEST_STRING = "AaBbCc 123 `~!#$%^\t\r\n";

		Assert.Equal(TEST_STRING, TEST_STRING.ToBytes(Encoding.UTF8).ToText(Encoding.UTF8));

		Assert.Equal("BbCc 1", TEST_STRING.ToBytes(Encoding.UTF8).ToText(Encoding.UTF8, 2, 6));

		Assert.Equal(TEST_STRING, ((ReadOnlySpan<byte>)TEST_STRING.ToBytes(Encoding.UTF8).AsSpan()).ToText(Encoding.UTF8));
	}

	[Fact]
	public void ToEnum()
	{
		Assert.Equal(StringComparison.Ordinal, nameof(StringComparison.Ordinal).ToEnum<StringComparison>());
		Assert.Equal(StringComparison.OrdinalIgnoreCase, nameof(StringComparison.OrdinalIgnoreCase).ToUpperInvariant().ToEnum<StringComparison>());
	}

	[Fact]
	public void TrimEnd()
	{
		const string TEST_STRING = "AaBbCc 123 `~!#$%^\t\r\n";

		Assert.Equal("AaB", TEST_STRING.TrimEnd("BCC 123 `~!#$%^\t\r\n"));
		Assert.NotEqual("AaB", TEST_STRING.TrimEnd("BCC 123 `~!#$%^\t\r\n", StringComparison.Ordinal));
		Assert.Equal(TEST_STRING, TEST_STRING.TrimEnd("******"));
	}

	[Fact]
	public void TrimStart()
	{
		const string TEST_STRING = "AaBbCc 123 `~!#$%^\t\r\n";

		Assert.Equal(" `~!#$%^\t\r\n", TEST_STRING.TrimStart("aabbcc 123"));
		Assert.NotEqual(" `~!#$%^\t\r\n", TEST_STRING.TrimStart("aabbcc 123", StringComparison.Ordinal));
		Assert.Equal(TEST_STRING, TEST_STRING.TrimStart("******"));
	}
}
