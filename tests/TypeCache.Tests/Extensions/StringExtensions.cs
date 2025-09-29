// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using TypeCache.Extensions;
using TypeCache.Testing;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class StringExtensions
{
	private const string TEST_STRING = "AaBbCc 123 `~!#$%^\t\r\n/\\&=";

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
	public void EqualsIgnoreCase()
	{
		Assert.True(TEST_STRING.EqualsIgnoreCase(TEST_STRING.ToLowerInvariant()));
		Assert.True(TEST_STRING.EqualsIgnoreCase(TEST_STRING.ToUpperInvariant()));
	}

	[Fact]
	public void FromBase64()
	{
		Assert.Equal(TEST_STRING, TEST_STRING.ToBase64(Encoding.ASCII).FromBase64(Encoding.ASCII));
		Assert.Equal(TEST_STRING, TEST_STRING.ToBase64().FromBase64());
	}

	[Fact]
	public void FromBase64Url()
	{
		Assert.Equal(TEST_STRING, TEST_STRING.ToBase64Url(Encoding.ASCII).FromBase64Url(Encoding.ASCII));
		Assert.Equal(TEST_STRING, TEST_STRING.ToBase64Url().FromBase64Url());
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
		Assert.Equal(TEST_STRING, " ".Join([TEST_STRING]));
	}

	[Fact]
	public void Mask()
	{
		Assert.Equal(string.Empty, (null as string).Mask());
		Assert.Equal("++++++ +++ `~!#$%^\t\r\n/\\&=", TEST_STRING.Mask('+'));
	}

	[Fact]
	public void MaskIgnoreCase()
	{
		Assert.Equal("++++++ 123 `~!#$%^\t\r\n/\\&=", TEST_STRING.MaskIgnoreCase('+', ["A", "b", "C"]));
	}

	[Fact]
	public void MaskOrdinal()
	{
		Assert.Equal("+aB++c 123 `~!#$%^\t\r\n/\\&=", TEST_STRING.MaskOrdinal('+', ["A", "b", "C"]));
	}

	[Fact]
	public void Reverse()
	{
		Assert.Equal(string.Empty, string.Empty.Reverse());
		Assert.Equal("321 cCbBaA", "AaBbCc 123".Reverse());
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
		//var loggerMock = Substitute.For<ILogger>();
		//ILogger loggerMock = (ILogger)(dynamic)new DynamicMock<ILogger>();
		var loggerMock = new Shadow<ILogger>().Instance;
		loggerMock.LogInformation("TEST");

		"AAA".ThrowIfBlank(logger: loggerMock);
		Assert.Throws<ArgumentOutOfRangeException>(() => (null as string).ThrowIfBlank(logger: loggerMock));
		Assert.Throws<ArgumentOutOfRangeException>(() => string.Empty.ThrowIfBlank());
		Assert.Throws<ArgumentOutOfRangeException>(() => "      ".ThrowIfBlank(logger: loggerMock));
	}

	[Fact]
	public void ToBase64()
	{
		Assert.Equal(TEST_STRING, TEST_STRING.ToBase64(Encoding.ASCII).FromBase64(Encoding.ASCII));
		Assert.Equal(TEST_STRING, TEST_STRING.ToBase64().FromBase64());
	}

	[Fact]
	public void ToBase64Url()
	{
		Assert.Equal(TEST_STRING, TEST_STRING.ToBase64Url(Encoding.ASCII).FromBase64Url(Encoding.ASCII));
		Assert.Equal(TEST_STRING, TEST_STRING.ToBase64Url().FromBase64Url());
	}

	[Fact]
	public void ToEnum()
	{
		Assert.Equal(StringComparison.Ordinal, nameof(StringComparison.Ordinal).ToEnum<StringComparison>());
		Assert.Equal(StringComparison.OrdinalIgnoreCase, nameof(StringComparison.OrdinalIgnoreCase).ToEnum<StringComparison>());
	}

	[Fact]
	public void ToEnumIgnoreCase()
	{
		Assert.Equal(StringComparison.Ordinal, nameof(StringComparison.Ordinal).ToLowerInvariant().ToEnumIgnoreCase<StringComparison>());
		Assert.Equal(StringComparison.OrdinalIgnoreCase, nameof(StringComparison.OrdinalIgnoreCase).ToUpperInvariant().ToEnumIgnoreCase<StringComparison>());
	}

	[Fact]
	public void ToIndex()
	{
		var index = "".ToIndex();
		Assert.Null(index);

		index = "AA".ToIndex();
		Assert.Null(index);

		index = "^".ToIndex();
		Assert.Null(index);

		index = "^3".ToIndex();
		Assert.Equal(^3, index);

		index = "55".ToIndex();
		Assert.Equal(55, index);
	}

	[Fact]
	public void ToRange()
	{
		var range = "..".ToRange();
		Assert.NotNull(range);
		Assert.Equal(Range.All, range);

		range = "2..6".ToRange();
		Assert.NotNull(range);
		Assert.Equal(2, range.Value.Start);
		Assert.False(range.Value.Start.IsFromEnd);
		Assert.Equal(6, range.Value.End);
		Assert.False(range.Value.End.IsFromEnd);

		range = "..^2".ToRange();
		Assert.NotNull(range);
		Assert.Equal(0, range.Value.Start);
		Assert.False(range.Value.Start.IsFromEnd);
		Assert.Equal(^2, range.Value.End);
		Assert.True(range.Value.End.IsFromEnd);

		range = "3".ToRange();
		Assert.NotNull(range);
		Assert.Equal(3, range.Value.Start);
		Assert.False(range.Value.Start.IsFromEnd);
		Assert.Equal(4, range.Value.End);
		Assert.False(range.Value.End.IsFromEnd);

		range = "^2..".ToRange();
		Assert.NotNull(range);
		Assert.Equal(^2.., range.Value);

		range = "^".ToRange();
		Assert.Null(range);

		range = "..^".ToRange();
		Assert.Null(range);

		range = "^..".ToRange();
		Assert.Null(range);

		range = "^..^".ToRange();
		Assert.Null(range);

		range = "0..^".ToRange();
		Assert.Null(range);

		range = "1..2..3..4".ToRange();
		Assert.Null(range);
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
	public void TrimEndIgnoreCase()
	{
		Assert.Equal("AaB", TEST_STRING.TrimEndIgnoreCase("BCC 123 `~!#$%^\t\r\n/\\&="));
		Assert.Equal(TEST_STRING, TEST_STRING.TrimEndIgnoreCase("******"));
	}

	[Fact]
	public void TrimEndOrdinal()
	{
		Assert.NotEqual("AaB", TEST_STRING.TrimEndOrdinal("BCC 123 `~!#$%^\t\r\n/\\&="));
	}

	[Fact]
	public void TrimStartIgnoreCase()
	{
		Assert.Equal(" `~!#$%^\t\r\n/\\&=", TEST_STRING.TrimStartIgnoreCase("aabbcc 123"));
		Assert.Equal(TEST_STRING, TEST_STRING.TrimStartOrdinal("******"));
	}

	[Fact]
	public void TrimStartOrdinal()
	{
		Assert.NotEqual(" `~!#$%^\t\r\n/\\&=", TEST_STRING.TrimStartOrdinal("aabbcc 123"));
	}
}
