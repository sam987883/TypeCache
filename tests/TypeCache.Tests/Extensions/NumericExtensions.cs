// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using System.Numerics;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class NumericExtensions
{
	[Fact]
	public void Abs()
	{
		Assert.Equal(long.Abs(-66L), (-66L).Abs());
	}

	[Fact]
	public void BitDecrement()
	{
		Assert.Equal(double.BitDecrement(-66.66), (-66.66).BitDecrement());
	}

	[Fact]
	public void BitIncrement()
	{
		Assert.Equal(double.BitIncrement(-66.66), (-66.66).BitIncrement());
	}

	[Fact]
	public void Ceiling()
	{
		Assert.Equal(double.Ceiling(-66.66), (-66.66).Ceiling());
	}

	[Fact]
	public void Factorial()
	{
		Assert.Throws<ArgumentOutOfRangeException>(() => (-1).Factorial());
		Assert.Equal(1UL, 0.Factorial());
		Assert.Equal(1UL, 1.Factorial());
		Assert.Equal(2UL, 2.Factorial());
		Assert.Equal(6UL, 3.Factorial());
		Assert.Equal(24UL, 4.Factorial());
		Assert.Equal(120UL, 5.Factorial());
	}

	[Fact]
	public void Floor()
	{
		Assert.Equal(double.Floor(-66.66), (-66.66).Floor());
	}

	[Fact]
	public void Max()
	{
		Assert.Equal(double.Max(-66.66, 99.99), (-66.66, 99.99).Max());
		Assert.Equal(Index.FromStart(99), (Index.FromStart(66), Index.FromStart(99)).Max());
		Assert.Equal(Index.FromEnd(66), (Index.FromEnd(66), Index.FromEnd(99)).Max());
	}

	[Fact]
	public void Min()
	{
		Assert.Equal(double.Min(-66.66, 99.99), (-66.66, 99.99).Min());
		Assert.Equal(Index.FromStart(66), (Index.FromStart(66), Index.FromStart(99)).Min());
		Assert.Equal(Index.FromEnd(99), (Index.FromEnd(66), Index.FromEnd(99)).Min());
	}

	[Fact]
	public void Repeat()
	{
		Assert.Equal(7, 1.Repeat(7).Count());
	}

	[Fact]
	public void Round()
	{
		Assert.Equal(double.Round(111.11), 111.11.Round());
		Assert.Equal(double.Round(111.1123456, 3), 111.1123456.Round(3));
		Assert.Equal(double.Round(111.1123456, MidpointRounding.AwayFromZero), 111.1123456.Round(MidpointRounding.AwayFromZero));
		Assert.Equal(double.Round(111.1123456, MidpointRounding.ToEven), 111.1123456.Round(MidpointRounding.ToEven));
		Assert.Equal(double.Round(111.1123456, MidpointRounding.ToNegativeInfinity), 111.1123456.Round(MidpointRounding.ToNegativeInfinity));
		Assert.Equal(double.Round(111.1123456, MidpointRounding.ToPositiveInfinity), 111.1123456.Round(MidpointRounding.ToPositiveInfinity));
		Assert.Equal(double.Round(111.1123456, MidpointRounding.ToZero), 111.1123456.Round(MidpointRounding.ToZero));
		Assert.Equal(double.Round(111.1123456, 5, MidpointRounding.AwayFromZero), 111.1123456.Round(5, MidpointRounding.AwayFromZero));
		Assert.Equal(double.Round(111.1123456, 5, MidpointRounding.ToEven), 111.1123456.Round(5, MidpointRounding.ToEven));
		Assert.Equal(double.Round(111.1123456, 5, MidpointRounding.ToNegativeInfinity), 111.1123456.Round(5, MidpointRounding.ToNegativeInfinity));
		Assert.Equal(double.Round(111.1123456, 5, MidpointRounding.ToPositiveInfinity), 111.1123456.Round(5, MidpointRounding.ToPositiveInfinity));
		Assert.Equal(double.Round(111.1123456, 5, MidpointRounding.ToZero), 111.1123456.Round(5, MidpointRounding.ToZero));
	}

	[Fact]
	public void Sign()
	{
		Assert.Equal(int.Sign(7), 7.Sign());
		Assert.Equal(int.Sign(-7), (-7).Sign());
	}

	[Fact]
	public void ToBoolean()
	{
		var bytes = new byte[3];
		Assert.Equal(BitConverter.ToBoolean(bytes), bytes.AsSpan().AsReadOnly().ToBoolean());
		Assert.Equal(BitConverter.ToBoolean(bytes), bytes.ToBoolean());
		Assert.Equal(BitConverter.ToBoolean(bytes, 1), bytes.ToBoolean(1));
	}

	[Fact]
	public void ToBytes()
	{
		Assert.Equal(BitConverter.GetBytes('a'), 'a'.ToBytes());
		Assert.Equal([(byte)(sbyte)123], ((sbyte)123).ToBytes());
		Assert.Equal(BitConverter.GetBytes((short)123), ((short)123).ToBytes());
		Assert.Equal(BitConverter.GetBytes(123), 123.ToBytes());
		Assert.Equal(BitConverter.GetBytes((nint)123), ((nint)123).ToBytes());
		Assert.Equal(BitConverter.GetBytes(123L), 123L.ToBytes());
		Assert.Equal(BitConverter.GetBytes((Int128)123), ((Int128)123).ToBytes());
		Assert.Equal(((BigInteger)123).ToByteArray(), ((BigInteger)123).ToBytes());
		Assert.Equal([(byte)123], ((byte)123).ToBytes());
		Assert.Equal(BitConverter.GetBytes((ushort)123), ((ushort)123).ToBytes());
		Assert.Equal(BitConverter.GetBytes(123U), 123U.ToBytes());
		Assert.Equal(BitConverter.GetBytes(123UL), 123UL.ToBytes());
		Assert.Equal(BitConverter.GetBytes((UInt128)123), ((UInt128)123).ToBytes());
		Assert.Equal(BitConverter.GetBytes((Half)1.23), ((Half)1.23).ToBytes());
		Assert.Equal(BitConverter.GetBytes(1.23F), 1.23F.ToBytes());
		Assert.Equal(BitConverter.GetBytes(1.23), 1.23.ToBytes());
		Assert.Equal(decimal.GetBits(1.23M).SelectMany(_ => _.ToBytes()).ToArray(), 1.23M.ToBytes());
	}

	[Fact]
	public void ToDouble()
	{
		Assert.Equal(BitConverter.Int64BitsToDouble(123456L), 123456L.ToDouble());
	}

	[Fact]
	public void ToHalf()
	{
		Assert.Equal(BitConverter.Int16BitsToHalf(12345), ((short)12345).ToHalf());
		Assert.Equal(BitConverter.UInt16BitsToHalf(12345), ((ushort)12345).ToHalf());
	}

	[Fact]
	public void ToInt16()
	{
		Assert.Equal(BitConverter.HalfToInt16Bits((Half)123), ((Half)123).ToInt16());
	}

	[Fact]
	public void ToInt32()
	{
		Assert.Equal(BitConverter.SingleToInt32Bits(123456F), 123456F.ToInt32());
	}

	[Fact]
	public void ToInt64()
	{
		Assert.Equal(BitConverter.DoubleToInt64Bits(123456789D), 123456789D.ToInt64());
	}

	[Fact]
	public void ToNumber()
	{
		var bytes = new byte[64];
		Assert.Equal(BitConverter.ToChar(bytes), bytes.ToNumber<char>());
		Assert.Equal(BitConverter.ToChar(bytes, 1), bytes.ToNumber<char>(1));
		Assert.Equal(BitConverter.ToDouble(bytes), bytes.AsSpan().AsReadOnly().ToNumber<double>());
		Assert.Equal(BitConverter.ToDouble(bytes), bytes.ToNumber<double>());
		Assert.Equal(BitConverter.ToDouble(bytes, 2), bytes.ToNumber<double>(2));
	}

	[Fact]
	public void ToSingle()
	{
		Assert.Equal(BitConverter.Int32BitsToSingle(123456), 123456.ToSingle());
		Assert.Equal(BitConverter.UInt32BitsToSingle(123456U), 123456U.ToSingle());
	}

	[Fact]
	public void ToText()
	{
		var bytes = new byte[64];
		Assert.Equal(BitConverter.ToString(bytes), bytes.AsSpan().AsReadOnly().ToText());
		Assert.Equal(BitConverter.ToString(bytes, 2, 4), bytes.AsSpan().AsReadOnly().ToText(2, 4));
		Assert.Equal(BitConverter.ToString(bytes), bytes.ToText());
		Assert.Equal(BitConverter.ToString(bytes, 2), bytes.ToText(2));
		Assert.Equal(BitConverter.ToString(bytes, 2, 4), bytes.ToText(2, 4));
	}

	[Fact]
	public void ToUInt16()
	{
		Assert.Equal(BitConverter.HalfToUInt16Bits((Half)123), ((Half)123).ToUInt16());
	}

	[Fact]
	public void ToUInt32()
	{
		Assert.Equal(BitConverter.SingleToUInt32Bits(123456F), 123456F.ToUInt32());
	}

	[Fact]
	public void ToUInt64()
	{
		Assert.Equal(BitConverter.DoubleToUInt64Bits(123456789D), 123456789D.ToUInt64());
	}
}
