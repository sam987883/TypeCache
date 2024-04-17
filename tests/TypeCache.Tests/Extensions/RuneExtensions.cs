// Copyright (c) 2021 Samuel Abraham

using System;
using System.Text;
using TypeCache.Extensions;
using Xunit;
using static System.Globalization.CultureInfo;

namespace TypeCache.Tests.Extensions;

public class RuneExtensions
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
		Assert.Equal(Rune.GetUnicodeCategory(new(value)), new Rune(value).GetUnicodeCategory());
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
		Assert.Equal(Rune.IsControl(new(value)), new Rune(value).IsControl());
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
		Assert.Equal(Rune.IsDigit(new(value)), new Rune(value).IsDigit());
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
		Assert.Equal(Rune.IsLetter(new(value)), new Rune(value).IsLetter());
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
		Assert.Equal(Rune.IsLetterOrDigit(new(value)), new Rune(value).IsLetterOrDigit());
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
		Assert.Equal(Rune.IsLower(new(value)), new Rune(value).IsLower());
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
		Assert.Equal(Rune.IsNumber(new(value)), new Rune(value).IsNumber());
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
		Assert.Equal(Rune.IsPunctuation(new(value)), new Rune(value).IsPunctuation());
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
		Assert.Equal(Rune.IsSeparator(new(value)), new Rune(value).IsSeparator());
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
		Assert.Equal(Rune.IsSymbol(new(value)), new Rune(value).IsSymbol());
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
		Assert.Equal(Rune.IsUpper(new(value)), new Rune(value).IsUpper());
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
		Assert.Equal(Rune.IsWhiteSpace(new(value)), new Rune(value).IsWhiteSpace());
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
		Assert.Equal(Rune.ToLower(new(value), InvariantCulture), new Rune(value).ToLower(InvariantCulture));
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
		Assert.Equal(Rune.ToLowerInvariant(new(value)), new Rune(value).ToLowerInvariant());
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
		Assert.Equal(Rune.GetNumericValue(new(value)), new Rune(value).ToNumber());
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
		Assert.Equal(Rune.ToUpper(new(value), InvariantCulture), new Rune(value).ToUpper(InvariantCulture));
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
		Assert.Equal(Rune.ToUpperInvariant(new(value)), new Rune(value).ToUpperInvariant());
	}
}
