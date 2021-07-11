// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions
{
	public class MathExtensions
	{
		[Fact]
		public void AbsoluteValue()
		{
			Assert.Equal(-(sbyte.MinValue + 1), (sbyte.MinValue + 1).AbsoluteValue());
			Assert.Equal(-(short.MinValue + 1), (short.MinValue + 1).AbsoluteValue());
			Assert.Equal(-(int.MinValue + 1), (int.MinValue + 1).AbsoluteValue());
			Assert.Equal(-(long.MinValue + 1), (long.MinValue + 1).AbsoluteValue());
			Assert.Equal(1F, (-1F).AbsoluteValue());
			Assert.Equal(1D, (-1D).AbsoluteValue());
			Assert.Equal(1M, (-1M).AbsoluteValue());
		}

		[Fact]
		public void BitDecrement()
		{
			Assert.Equal(1D, 1D.BitIncrement().BitDecrement());
		}

		[Fact]
		public void BitIncrement()
		{
			Assert.Equal(1D, 1D.BitDecrement().BitIncrement());
		}

		[Fact]
		public void Ceiling()
		{
			Assert.Equal(2D, 1.111D.Ceiling());
			Assert.Equal(2M, 1.111M.Ceiling());
		}

		[Fact]
		public void Floor()
		{
			Assert.Equal(1D, 1.111D.Floor());
			Assert.Equal(1M, 1.111M.Floor());
		}

		[Fact]
		public void Round()
		{
			Assert.Equal(123D, 123.456D.Round());
			Assert.Equal(123.5D, 123.456D.Round(1));
			Assert.Equal(123D, 123.456D.Round(MidpointRounding.AwayFromZero));
			Assert.Equal(123.46D, 123.456D.Round(2, MidpointRounding.ToEven));
			Assert.Equal(123M, 123.456M.Round());
			Assert.Equal(123.5M, 123.456M.Round(1));
			Assert.Equal(123M, 123.456M.Round(MidpointRounding.AwayFromZero));
			Assert.Equal(123.46M, 123.456M.Round(2, MidpointRounding.ToEven));
		}

		[Fact]
		public void Sign()
		{
			Assert.Equal(1, sbyte.MaxValue.Sign());
			Assert.Equal(-1, sbyte.MinValue.Sign());
			Assert.Equal(1, short.MaxValue.Sign());
			Assert.Equal(-1, short.MinValue.Sign());
			Assert.Equal(1, int.MaxValue.Sign());
			Assert.Equal(-1, int.MinValue.Sign());
			Assert.Equal(1, long.MaxValue.Sign());
			Assert.Equal(-1, long.MinValue.Sign());
			Assert.Equal(-1, -666F.Sign());
			Assert.Equal(1, 666F.Sign());
			Assert.Equal(-1, -666D.Sign());
			Assert.Equal(1, 666D.Sign());
			Assert.Equal(-1, -666M.Sign());
			Assert.Equal(1, 666M.Sign());
		}

		[Fact]
		public void Truncate()
		{
			Assert.Equal(-123D, -123.456D.Truncate());
			Assert.Equal(123M, 123.456M.Truncate());
		}
	}
}
