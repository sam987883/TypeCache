// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Globalization;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class CharExtensions
{
	[Fact]
	public void IsControl()
	{
		Assert.False('f'.IsControl());
	}

	[Fact]
	public void IsDigit()
	{
		Assert.False('f'.IsDigit());
		Assert.True('6'.IsDigit());
	}

	[Fact]
	public void IsHighSurrogate()
	{
		Assert.False('f'.IsHighSurrogate());
	}

	[Fact]
	public void IsLetter()
	{
		Assert.False('6'.IsLetter());
		Assert.True('f'.IsLetter());
	}

	[Fact]
	public void IsLetterOrDigit()
	{
		Assert.True('f'.IsLetterOrDigit());
		Assert.True('6'.IsLetterOrDigit());
	}

	[Fact]
	public void IsLower()
	{
		Assert.True('f'.IsLower());
		Assert.False('F'.IsLower());
	}

	[Fact]
	public void IsLowSurrogate()
	{
		Assert.False('f'.IsLowSurrogate());
	}

	[Fact]
	public void IsNumber()
	{
		Assert.True('6'.IsNumber());
		Assert.False('f'.IsNumber());
	}

	[Fact]
	public void IsPunctuation()
	{
		Assert.False('6'.IsPunctuation());
		Assert.True('!'.IsPunctuation());
	}

	[Fact]
	public void IsSeparator()
	{
		Assert.False('f'.IsSeparator());
		Assert.True(' '.IsSeparator());
	}

	[Fact]
	public void IsSurrogate()
	{
		Assert.False('f'.IsSurrogate());
	}

	[Fact]
	public void IsSymbol()
	{
		Assert.False('f'.IsSymbol());
		Assert.False('#'.IsSymbol());
	}

	[Fact]
	public void IsUpper()
	{
		Assert.False('f'.IsUpper());
		Assert.True('F'.IsUpper());
	}

	[Fact]
	public void IsWhiteSpace()
	{
		Assert.False('f'.IsWhiteSpace());
		Assert.True(' '.IsWhiteSpace());
	}

	[Fact]
	public void Join()
	{

		Assert.Equal("A,b,C,1,2,3", ','.Join("A", "b", "C", "1", "2", "3"));

		Assert.Equal("A.b.C.1.2.3", '.'.Join((IEnumerable<string>)new[] { "A", "b", "C", "1", "2", "3" }));

		Assert.Equal("A;b;C;1;2;3", ';'.Join('A', 'b', 'C', 1, 2, 3));

		Assert.Equal("A|b|C|1|2|3", '|'.Join((IEnumerable<object>)new object[] { 'A', 'b', 'C', 1, 2, 3 }));
	}

	[Fact]
	public void ToLower()
	{
		Assert.Equal('f', 'F'.ToLowerInvariant());
	}

	[Fact]
	public void ToNumber()
	{
		Assert.Equal(0D, '0'.ToNumber());
	}

	[Fact]
	public void ToUpper()
	{
		Assert.Equal('F', 'f'.ToUpperInvariant());
	}

	[Fact]
	public void ToUnicodeCategory()
	{
		Assert.Equal(UnicodeCategory.LowercaseLetter, 'f'.GetUnicodeCategory());
		Assert.Equal(UnicodeCategory.UppercaseLetter, 'F'.GetUnicodeCategory());
	}
}
