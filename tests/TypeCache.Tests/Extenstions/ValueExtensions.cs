// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions
{
	public class ValueExtensions
	{
		[Fact]
		public void IsReverse()
		{
			Assert.True(new Range(Index.FromStart(20), Index.FromStart(3)).IsReverse());
			Assert.False(new Range(Index.FromStart(6), Index.FromStart(6)).IsReverse());
			Assert.False(new Range(Index.FromStart(3), Index.FromStart(20)).IsReverse());
		}

		[Fact]
		public void Normalize()
		{
			Assert.Equal(Index.FromStart(15), Index.FromEnd(5).Normalize(20));
			Assert.Equal(Index.FromStart(5), Index.FromStart(5).Normalize(20));

			Assert.Equal(new Range(Index.FromStart(5), Index.FromStart(15)), new Range(Index.FromEnd(15), Index.FromEnd(5)).Normalize(20));
			Assert.Equal(new Range(Index.FromStart(5), Index.FromStart(15)), new Range(Index.FromStart(5), Index.FromStart(15)).Normalize(20));
		}

		[Fact]
		public void Range()
		{
			Assert.Equal(new[] { -2, -1, 0, 1, 2 }, (-2).Range(5).ToArray());
			Assert.Equal(new[] { 2, 2, 2, 2, 2, 2 }, 2.Range(6, 0).ToArray());
			Assert.Equal(new[] { 9, 6, 3, 0, -3, -6 }, 9.Range(6, -3).ToArray());
			Assert.Equal(Array<int>.Empty, 123.Range(0, 123).ToArray());
		}

		[Fact]
		public void Repeat()
		{
			Assert.Equal(new[] { 'f', 'f', 'f', 'f', 'f', 'f' }, 'f'.Repeat(6).ToArray());
			Assert.Equal(Array<int>.Empty, 123.Repeat(0).ToArray());
			Assert.Equal(Array<int>.Empty, 123.Repeat(-18).ToArray());
		}

		[Fact]
		public void Swap()
		{
			var a = 123;
			var b = -456;
			b.Swap(ref a);
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

		[Fact]
		public void Values()
		{
			Assert.Equal(new[] { 5, 6, 7 }, new Range(Index.FromStart(5), Index.FromStart(8)).Values().ToArray());
			Assert.Equal(new[] { 7, 6, 5 }, new Range(Index.FromStart(7), Index.FromStart(4)).Values().ToArray());
			Assert.Equal(new[] { 0, 1, 2, 3 }, new Range(Index.FromStart(0), Index.FromStart(4)).Values().ToArray());
			Assert.Equal(new[] { 4, 3, 2, 1 }, new Range(Index.FromStart(4), Index.FromStart(0)).Values().ToArray());
		}
	}
}
