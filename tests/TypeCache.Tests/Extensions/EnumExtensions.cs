// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
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
		Assert.Equal(3, TestEnum.TestValue2.Attributes().Count());
	}

	[Fact]
	public void Hex()
	{
		Assert.Equal(TestEnum.TestValue2.ToString("X"), TestEnum.TestValue2.Hex());
	}

	[Fact]
	public void IsPrimitive()
	{
		Assert.Equal(typeof(bool).IsPrimitive, ScalarType.Boolean.IsPrimitive());
		Assert.Equal(typeof(char).IsPrimitive, ScalarType.Char.IsPrimitive());
		Assert.Equal(typeof(sbyte).IsPrimitive, ScalarType.SByte.IsPrimitive());
		Assert.Equal(typeof(short).IsPrimitive, ScalarType.Int16.IsPrimitive());
		Assert.Equal(typeof(int).IsPrimitive, ScalarType.Int32.IsPrimitive());
		Assert.Equal(typeof(long).IsPrimitive, ScalarType.Int64.IsPrimitive());
		Assert.Equal(typeof(nint).IsPrimitive, ScalarType.IntPtr.IsPrimitive());
		Assert.Equal(!typeof(Int128).IsPrimitive, ScalarType.Int128.IsPrimitive());
		Assert.Equal(typeof(byte).IsPrimitive, ScalarType.Byte.IsPrimitive());
		Assert.Equal(typeof(ushort).IsPrimitive, ScalarType.UInt16.IsPrimitive());
		Assert.Equal(typeof(uint).IsPrimitive, ScalarType.UInt32.IsPrimitive());
		Assert.Equal(typeof(ulong).IsPrimitive, ScalarType.UInt64.IsPrimitive());
		Assert.Equal(typeof(nuint).IsPrimitive, ScalarType.UIntPtr.IsPrimitive());
		Assert.Equal(!typeof(UInt128).IsPrimitive, ScalarType.UInt128.IsPrimitive());
		Assert.Equal(typeof(Half).IsPrimitive, ScalarType.Half.IsPrimitive());
		Assert.Equal(typeof(float).IsPrimitive, ScalarType.Single.IsPrimitive());
		Assert.Equal(typeof(double).IsPrimitive, ScalarType.Double.IsPrimitive());
		Assert.Equal(!typeof(decimal).IsPrimitive, ScalarType.Decimal.IsPrimitive());
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
