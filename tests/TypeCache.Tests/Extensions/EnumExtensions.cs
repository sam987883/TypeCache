// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class EnumExtensions
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	private class TestAttribute : Attribute
	{
	}

	[Flags]
	private enum TestEnum
	{
		TestValue1 = 1,
		[TestAttribute]
		[TestAttribute]
		[TestAttribute]
		TestValue2 = 2,
		TestValue3 = 4,
		TestValue4 = 8,
	}

	[Fact]
	public void Attributes()
	{
		Assert.Equal(3, TestEnum.TestValue2.Attributes().Count);
	}

	[Fact]
	public void Hex()
	{
		Assert.Equal(TestEnum.TestValue2.ToString("X"), TestEnum.TestValue2.Hex());
	}

	[Fact]
	public void Name()
	{
		Assert.Equal(TestEnum.TestValue2.ToString("F"), TestEnum.TestValue2.Name());
		Assert.Equal((TestEnum.TestValue2 | TestEnum.TestValue3).ToString("F"), (TestEnum.TestValue2 | TestEnum.TestValue3).Name());
	}

	[Fact]
	public void Number()
	{
		Assert.Equal(TestEnum.TestValue2.ToString("D"), TestEnum.TestValue2.Number());
	}

	[Fact]
	public void ToStringComparer()
	{
		Assert.Equal(StringComparer.CurrentCulture, StringComparison.CurrentCulture.ToStringComparer());
		Assert.Equal(StringComparer.CurrentCultureIgnoreCase, StringComparison.CurrentCultureIgnoreCase.ToStringComparer());
		Assert.Equal(StringComparer.InvariantCulture, StringComparison.InvariantCulture.ToStringComparer());
		Assert.Equal(StringComparer.InvariantCultureIgnoreCase, StringComparison.InvariantCultureIgnoreCase.ToStringComparer());
		Assert.Equal(StringComparer.Ordinal, StringComparison.Ordinal.ToStringComparer());
		Assert.Equal(StringComparer.OrdinalIgnoreCase, StringComparison.OrdinalIgnoreCase.ToStringComparer());
		Assert.Throws<ArgumentException>(() => ((StringComparison)666).ToStringComparer());
	}
}
