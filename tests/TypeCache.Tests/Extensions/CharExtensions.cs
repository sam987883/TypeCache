// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using TypeCache.Extensions;
using Xunit;
using static System.Globalization.CultureInfo;

namespace TypeCache.Tests.Extensions;

public class CharExtensions
{
	[Fact]
	public void Join()
	{
		Assert.Equal("A,b,C,1,2,3", ','.Join("A", "b", "C", "1", "2", "3"));
		Assert.Equal("A.b.C.1.2.3", '.'.Join(new[] { "A", "b", "C", "1", "2", "3" }.AsEnumerable()));
		Assert.Equal("A;b;C;1;2;3", ';'.Join('A', 'b', 'C', 1, 2, 3));
		Assert.Equal("A|b|C|1|2|3", '|'.Join(new object[] { 'A', 'b', 'C', 1, 2, 3 }.AsEnumerable()));
	}

	[Theory]
	[InlineData(' ')]
	[InlineData('1')]
	[InlineData('0')]
	[InlineData('A')]
	[InlineData('z')]
	[InlineData('.')]
	[InlineData('/')]
	[InlineData('\\')]
	[InlineData('\n')]
	[InlineData('`')]
	[InlineData('~')]
	[InlineData('\t')]
	[InlineData('?')]
	[InlineData('*')]
	public void TestExtensionsFor(char value)
	{
		Assert.Equal(char.GetUnicodeCategory(value), value.GetUnicodeCategory());
		Assert.Equal(char.IsAscii(value), value.IsAscii());
		Assert.Equal(char.IsAsciiDigit(value), value.IsAsciiDigit());
		Assert.Equal(char.IsAsciiHexDigit(value), value.IsAsciiHexDigit());
		Assert.Equal(char.IsAsciiHexDigitLower(value), value.IsAsciiHexDigitLower());
		Assert.Equal(char.IsAsciiHexDigitUpper(value), value.IsAsciiHexDigitUpper());
		Assert.Equal(char.IsAsciiLetter(value), value.IsAsciiLetter());
		Assert.Equal(char.IsAsciiLetterLower(value), value.IsAsciiLetterLower());
		Assert.Equal(char.IsAsciiLetterOrDigit(value), value.IsAsciiLetterOrDigit());
		Assert.Equal(char.IsAsciiLetterUpper(value), value.IsAsciiLetterUpper());
		Assert.Equal(char.IsControl(value), value.IsControl());
		Assert.Equal(char.IsDigit(value), value.IsDigit());
		Assert.Equal(char.IsHighSurrogate(value), value.IsHighSurrogate());
		Assert.Equal(char.IsLetter(value), value.IsLetter());
		Assert.Equal(char.IsLetterOrDigit(value), value.IsLetterOrDigit());
		Assert.Equal(char.IsLower(value), value.IsLower());
		Assert.Equal(char.IsLowSurrogate(value), value.IsLowSurrogate());
		Assert.Equal(char.IsNumber(value), value.IsNumber());
		Assert.Equal(char.IsPunctuation(value), value.IsPunctuation());
		Assert.Equal(char.IsSeparator(value), value.IsSeparator());
		Assert.Equal(char.IsSurrogate(value), value.IsSurrogate());
		Assert.Equal(char.IsSymbol(value), value.IsSymbol());
		Assert.Equal(char.IsUpper(value), value.IsUpper());
		Assert.Equal(char.IsWhiteSpace(value), value.IsWhiteSpace());
		Assert.Equal(char.ToLower(value), value.ToLower());
		Assert.Equal(char.ToLower(value, InvariantCulture), value.ToLower(InvariantCulture));
		Assert.Equal(char.ToLowerInvariant(value), value.ToLowerInvariant());
		Assert.Equal(char.GetNumericValue(value), value.ToNumber());
		Assert.Equal(char.ToUpper(value), value.ToUpper());
		Assert.Equal(char.ToUpper(value, InvariantCulture), value.ToUpper(InvariantCulture));
		Assert.Equal(char.ToUpperInvariant(value), value.ToUpperInvariant());
	}
}
