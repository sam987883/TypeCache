// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using TypeCache.Extensions;
using TypeCache.Reflection;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class EnumExtensions
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	private class TestAttribute : Attribute
	{
	}

	[Flags]
	public enum TestEnum
	{
		TestValue1 = 4,
		[TestAttribute]
		[TestAttribute]
		[TestAttribute]
		TestValue2 = 8,
		TestValue3 = 16,
		TestValue4 = 32,
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
	public void IsDefined()
	{
		Assert.True(TestEnum.TestValue3.IsDefined());
		Assert.False(((TestEnum)2).IsDefined());
		Assert.True((TestEnum.TestValue1 | TestEnum.TestValue2 | TestEnum.TestValue3).IsDefined());
		Assert.True((TestEnum.TestValue2 | (TestEnum)66).IsDefined());
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

	[Theory]
	[InlineData(TestEnum.TestValue1, TestEnum.TestValue1)]
	[InlineData(TestEnum.TestValue2, TestEnum.TestValue2)]
	[InlineData(TestEnum.TestValue3, TestEnum.TestValue3)]
	[InlineData(TestEnum.TestValue4, TestEnum.TestValue4)]
	public void ThrowIfEqual(TestEnum expected, TestEnum actual)
	{
		expected.ThrowIfEqual(default);
		actual.ThrowIfEqual(default);
		Assert.Throws<ArgumentOutOfRangeException>(() => expected.ThrowIfEqual(actual));
	}

	[Theory]
	[InlineData(TestEnum.TestValue1, TestEnum.TestValue2)]
	[InlineData(TestEnum.TestValue2, TestEnum.TestValue3)]
	[InlineData(TestEnum.TestValue3, TestEnum.TestValue4)]
	[InlineData(TestEnum.TestValue1, default)]
	public void ThrowIfNotEqual(TestEnum expected, TestEnum actual)
	{
		expected.ThrowIfNotEqual(expected);
		actual.ThrowIfNotEqual(actual);
		Assert.Throws<ArgumentOutOfRangeException>(() => expected.ThrowIfNotEqual(actual));
	}

	[Fact]
	public void ToComparer()
	{
		Assert.Equal(StringComparer.CurrentCulture, StringComparison.CurrentCulture.ToComparer());
		Assert.Equal(StringComparer.CurrentCultureIgnoreCase, StringComparison.CurrentCultureIgnoreCase.ToComparer());
		Assert.Equal(StringComparer.InvariantCulture, StringComparison.InvariantCulture.ToComparer());
		Assert.Equal(StringComparer.InvariantCultureIgnoreCase, StringComparison.InvariantCultureIgnoreCase.ToComparer());
		Assert.Equal(StringComparer.Ordinal, StringComparison.Ordinal.ToComparer());
		Assert.Equal(StringComparer.OrdinalIgnoreCase, StringComparison.OrdinalIgnoreCase.ToComparer());
		Assert.Throws<ArgumentException>(() => ((StringComparison)666).ToComparer());
	}

	[Fact]
	public void IsConcurrent()
	{
		Enum.GetValues<CollectionType>().ForEach(_ =>
		{
			if (_.Name().StartsWith("Concurrent"))
				Assert.True(_.IsConcurrent());
			else
				Assert.False(_.IsConcurrent());
		});
	}

	[Fact]
	public void IsDictionary()
	{
		Enum.GetValues<CollectionType>().ForEach(_ =>
		{
			if (_ is CollectionType.Hashtable
				|| _ is CollectionType.KeyedCollection
				|| _ is CollectionType.NameObjectCollection
				|| _ is CollectionType.NameValueCollection
				|| _ is CollectionType.SortedList
				|| _.Name().EndsWith("Dictionary"))
				Assert.True(_.IsDictionary());
			else
				Assert.False(_.IsDictionary());
		});
	}

	[Fact]
	public void IsEnumUnderlyingType()
	{
		Enum.GetValues<ScalarType>().ForEach(_ =>
		{
			if (_ is ScalarType.SByte
				|| _ is ScalarType.Int16
				|| _ is ScalarType.Int32
				|| _ is ScalarType.Int64
				|| _ is ScalarType.Byte
				|| _ is ScalarType.UInt16
				|| _ is ScalarType.UInt32
				|| _ is ScalarType.UInt64)
				Assert.True(_.IsEnumUnderlyingType());
			else
				Assert.False(_.IsEnumUnderlyingType());
		});
	}

	[Fact]
	public void IsFrozen()
	{
		Enum.GetValues<CollectionType>().ForEach(_ =>
			Assert.Equal(_.Name().StartsWith("Frozen"), _.IsFrozen()));
	}

	[Fact]
	public void IsImmutable()
	{
		Enum.GetValues<CollectionType>().ForEach(_ =>
			Assert.Equal(_.Name().StartsWith("Immutable"), _.IsImmutable()));
	}

	[Fact]
	public void IsPrimitive()
	{
		Enum.GetValues<ScalarType>().ForEach(_ =>
		{
			if (_ is ScalarType.Boolean
				|| _ is ScalarType.Char
				|| _ is ScalarType.SByte
				|| _ is ScalarType.Int16
				|| _ is ScalarType.Int32
				|| _ is ScalarType.Int64
				|| _ is ScalarType.IntPtr
				|| _ is ScalarType.Byte
				|| _ is ScalarType.UInt16
				|| _ is ScalarType.UInt32
				|| _ is ScalarType.UInt64
				|| _ is ScalarType.UIntPtr
				|| _ is ScalarType.Single
				|| _ is ScalarType.Double)
				Assert.True(_.IsPrimitive());
			else
				Assert.False(_.IsPrimitive());
		});
	}

	[Fact]
	public void IsQueue()
	{
		Enum.GetValues<CollectionType>().ForEach(_ =>
			Assert.Equal(_.Name().EndsWith("Queue"), _.IsQueue()));
	}

	[Fact]
	public void IsReadOnly()
	{
		Enum.GetValues<CollectionType>().ForEach(_ =>
			Assert.Equal(_.Name().StartsWith("ReadOnly"), _.IsReadOnly()));
	}

	[Fact]
	public void IsSet()
	{
		Enum.GetValues<CollectionType>().ForEach(_ =>
			Assert.Equal(_.Name().EndsWith("Set"), _.IsSet()));
	}

	[Fact]
	public void IsStack()
	{
		Enum.GetValues<CollectionType>().ForEach(_ =>
			Assert.Equal(_.Name().EndsWith("Stack"), _.IsStack()));
	}
}
