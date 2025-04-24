// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using TypeCache.Extensions;
using TypeCache.Utilities;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class ValueExtensions
{
	[Fact]
	public void Normalize()
	{
		Assert.Equal(Index.FromStart(15), Index.FromEnd(5).FromStart(20));
		Assert.Equal(Index.FromStart(5), Index.FromStart(5).FromStart(20));
	}

	[Fact]
	public void Repeat()
	{
		Assert.Equal(new[] { 'f', 'f', 'f', 'f', 'f', 'f' }, 'f'.Repeat(6).ToArray());
		Assert.Equal(Array<int>.Empty, 123.Repeat(0).ToArray());
		Assert.Equal(Enumerable<int>.Empty, 123.Repeat(-18));
	}

	[Fact]
	public void Swap()
	{
		var a = 123;
		var b = -456;
		b.Swap(ref a);

		Assert.Equal(-456, a);
		Assert.Equal(123, b);
	}

	[Fact]
	public void ToBytes()
	{
		Assert.Equal(16, (-999999.9999M).ToBytes().Length);
		Assert.Equal(16, 999999.9999M.ToBytes().Length);

		Assert.NotEmpty(decimal.MinValue.ToBytes());
		Assert.NotEmpty(decimal.MaxValue.ToBytes());
	}

	[Fact]
	public void ToDouble()
	{
		Assert.Equal(double.MinValue, double.MinValue.ToInt64().ToDouble());
		Assert.Equal(double.MaxValue, double.MaxValue.ToInt64().ToDouble());
	}

	[Fact]
	public void ToSingle()
	{
		Assert.Equal(float.MinValue, float.MinValue.ToInt32().ToSingle());
		Assert.Equal(float.MaxValue, float.MaxValue.ToInt32().ToSingle());
	}
}
