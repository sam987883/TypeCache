 // Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using TypeCache.Extensions;
using Xunit;
using static System.Globalization.CultureInfo;

namespace TypeCache.Tests.Extensions;

public class CharExtensions
{
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
	public void GetUnicodeCategory(char value)
	{
		Assert.Equal(char.GetUnicodeCategory(value), value.GetUnicodeCategory());
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
	public void IsAscii(char value)
	{
		Assert.Equal(char.IsAscii(value), value.IsAscii());
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
	public void IsAsciiDigit(char value)
	{
		Assert.Equal(char.IsAsciiDigit(value), value.IsAsciiDigit());
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
	public void IsAsciiHexDigit(char value)
	{
		Assert.Equal(char.IsAsciiHexDigit(value), value.IsAsciiHexDigit());
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
	public void IsAsciiHexDigitLower(char value)
	{
		Assert.Equal(char.IsAsciiHexDigitLower(value), value.IsAsciiHexDigitLower());
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
	public void IsAsciiHexDigitUpper(char value)
	{
		Assert.Equal(char.IsAsciiHexDigitUpper(value), value.IsAsciiHexDigitUpper());
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
	public void IsAsciiLetter(char value)
	{
		Assert.Equal(char.IsAsciiLetter(value), value.IsAsciiLetter());
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
	public void IsAsciiLetterLower(char value)
	{
		Assert.Equal(char.IsAsciiLetterLower(value), value.IsAsciiLetterLower());
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
	public void IsAsciiLetterOrDigit(char value)
	{
		Assert.Equal(char.IsAsciiLetterOrDigit(value), value.IsAsciiLetterOrDigit());
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
	public void IsAsciiLetterUpper(char value)
	{
		Assert.Equal(char.IsAsciiLetterUpper(value), value.IsAsciiLetterUpper());
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
	public void IsControl(char value)
	{
		Assert.Equal(char.IsControl(value), value.IsControl());
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
	public void IsDigit(char value)
	{
		Assert.Equal(char.IsDigit(value), value.IsDigit());
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
	public void IsHighSurrogate(char value)
	{
		Assert.Equal(char.IsHighSurrogate(value), value.IsHighSurrogate());
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
	public void IsLetter(char value)
	{
		Assert.Equal(char.IsLetter(value), value.IsLetter());
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
	public void IsLetterOrDigit(char value)
	{
		Assert.Equal(char.IsLetterOrDigit(value), value.IsLetterOrDigit());
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
	public void IsLower(char value)
	{
		Assert.Equal(char.IsLower(value), value.IsLower());
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
	public void IsLowSurrogate(char value)
	{
		Assert.Equal(char.IsLowSurrogate(value), value.IsLowSurrogate());
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
	public void IsNumber(char value)
	{
		Assert.Equal(char.IsNumber(value), value.IsNumber());
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
	public void IsPunctuation(char value)
	{
		Assert.Equal(char.IsPunctuation(value), value.IsPunctuation());
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
	public void IsSeparator(char value)
	{
		Assert.Equal(char.IsSeparator(value), value.IsSeparator());
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
	public void IsSurrogate(char value)
	{
		Assert.Equal(char.IsSurrogate(value), value.IsSurrogate());
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
	public void IsSymbol(char value)
	{
		Assert.Equal(char.IsSymbol(value), value.IsSymbol());
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
	public void IsUpper(char value)
	{
		Assert.Equal(char.IsUpper(value), value.IsUpper());
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
	public void IsWhiteSpace(char value)
	{
		Assert.Equal(char.IsWhiteSpace(value), value.IsWhiteSpace());
	}

	[Fact]
	public void Join()
	{
		Assert.Equal("A,b,C,1,2,3", ','.Join(["A", "b", "C", "1", "2", "3"]));
		Assert.Equal("A.b.C.1.2.3", '.'.Join(new[] { "A", "b", "C", "1", "2", "3" }.AsEnumerable()));
		Assert.Equal("A;b;C;1;2;3", ';'.Join(['A', 'b', 'C', 1, 2, 3]));
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
	public void ToLower(char value)
	{
		Assert.Equal(char.ToLower(value), value.ToLower());
		Assert.Equal(char.ToLower(value, InvariantCulture), value.ToLower(InvariantCulture));
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
	public void ToLowerInvariant(char value)
	{
		Assert.Equal(char.ToLowerInvariant(value), value.ToLowerInvariant());
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
	public void ToNumber(char value)
	{
		Assert.Equal(char.GetNumericValue(value), value.ToNumber());
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
	public void ToUpper(char value)
	{
		Assert.Equal(char.ToUpper(value), value.ToUpper());
		Assert.Equal(char.ToUpper(value, InvariantCulture), value.ToUpper(InvariantCulture));
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
	public void ToUpperInvariant(char value)
	{
		Assert.Equal(char.ToUpperInvariant(value), value.ToUpperInvariant());
	}
}
