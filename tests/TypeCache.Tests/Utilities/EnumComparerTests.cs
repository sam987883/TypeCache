// Copyright (c) 2021 Samuel Abraham

using TypeCache.Utilities;
using Xunit;

namespace TypeCache.Tests.Utilities;

public class EnumComparerTests
{
	private enum TestEnum
	{
		First = 0,
		Second = 1,
		Third = 2
	}

	[Fact]
	public void Compare_SameValue()
	{
		var comparer = new EnumComparer<TestEnum>();

		var result = comparer.Compare(TestEnum.First, TestEnum.First);

		Assert.Equal(0, result);
	}

	[Fact]
	public void Compare_FirstLess()
	{
		var comparer = new EnumComparer<TestEnum>();

		var result = comparer.Compare(TestEnum.First, TestEnum.Second);

		Assert.True(result < 0);
	}

	[Fact]
	public void Compare_FirstGreater()
	{
		var comparer = new EnumComparer<TestEnum>();

		var result = comparer.Compare(TestEnum.Third, TestEnum.First);

		Assert.True(result > 0);
	}

	[Fact]
	public void Equals_SameValue()
	{
		var comparer = new EnumComparer<TestEnum>();

		var result = comparer.Equals(TestEnum.First, TestEnum.First);

		Assert.True(result);
	}

	[Fact]
	public void Equals_DifferentValue()
	{
		var comparer = new EnumComparer<TestEnum>();

		var result = comparer.Equals(TestEnum.First, TestEnum.Second);

		Assert.False(result);
	}

	[Fact]
	public void EnumComparer_GetHashCode()
	{
		var comparer = new EnumComparer<TestEnum>();

		var hashCode = comparer.GetHashCode(TestEnum.Third);

		Assert.NotEqual(0, hashCode);
	}
}
