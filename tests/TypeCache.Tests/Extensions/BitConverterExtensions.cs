// Copyright (c) 2021 Samuel Abraham

using System;
using System.Text;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class BitConverterExtensions
{
	private readonly byte[] _Bytes = new byte[64];

	[Fact]
	public void GetBytes()
	{
		Assert.Equal(BitConverter.GetBytes(true), true.GetBytes());
		Assert.Equal(BitConverter.GetBytes(false), false.GetBytes());
		Assert.Equal(BitConverter.GetBytes('a'), 'a'.GetBytes());
		Assert.Equal(BitConverter.GetBytes(1.23), 1.23.GetBytes());
		Assert.Equal(BitConverter.GetBytes(1.23F), 1.23F.GetBytes());
		Assert.Equal(BitConverter.GetBytes((Half)1.23), ((Half)1.23).GetBytes());
		Assert.Equal(BitConverter.GetBytes(123), 123.GetBytes());
		Assert.Equal(BitConverter.GetBytes(123L), 123L.GetBytes());
		Assert.Equal(BitConverter.GetBytes((short)123), ((short)123).GetBytes());
		Assert.Equal(BitConverter.GetBytes(123U), 123U.GetBytes());
		Assert.Equal(BitConverter.GetBytes(123UL), 123UL.GetBytes());
		Assert.Equal(BitConverter.GetBytes((ushort)123), ((ushort)123).GetBytes());
	}

	[Fact]
	public void ToBoolean()
	{
		Assert.Equal(BitConverter.ToBoolean(this._Bytes), this._Bytes.AsSpan().AsReadOnly().ToBoolean());
		Assert.Equal(BitConverter.ToBoolean(this._Bytes), this._Bytes.ToBoolean());
		Assert.Equal(BitConverter.ToBoolean(this._Bytes, 1), this._Bytes.ToBoolean(1));
	}

	[Fact]
	public void ToChar()
	{
		Assert.Equal(BitConverter.ToChar(this._Bytes), this._Bytes.AsSpan().AsReadOnly().ToChar());
		Assert.Equal(BitConverter.ToChar(this._Bytes, 1), this._Bytes.ToChar(1));
	}

	[Fact]
	public void ToDouble()
	{
		Assert.Equal(BitConverter.ToDouble(this._Bytes), this._Bytes.AsSpan().AsReadOnly().ToDouble());
		Assert.Equal(BitConverter.ToDouble(this._Bytes), this._Bytes.ToDouble());
		Assert.Equal(BitConverter.ToDouble(this._Bytes, 2), this._Bytes.ToDouble(2));
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
		Assert.Equal(BitConverter.ToInt16(this._Bytes), this._Bytes.AsSpan().AsReadOnly().ToInt16());
		Assert.Equal(BitConverter.ToInt16(this._Bytes), this._Bytes.ToInt16());
		Assert.Equal(BitConverter.ToInt16(this._Bytes, 2), this._Bytes.ToInt16(2));
		Assert.Equal(BitConverter.HalfToInt16Bits((Half)123), ((Half)123).ToInt16());
	}

	[Fact]
	public void ToInt32()
	{
		Assert.Equal(BitConverter.ToInt32(this._Bytes), this._Bytes.AsSpan().AsReadOnly().ToInt32());
		Assert.Equal(BitConverter.ToInt32(this._Bytes), this._Bytes.ToInt32());
		Assert.Equal(BitConverter.ToInt32(this._Bytes, 2), this._Bytes.ToInt32(2));
		Assert.Equal(BitConverter.SingleToInt32Bits(123456F), 123456F.ToInt32());
	}

	[Fact]
	public void ToInt64()
	{
		Assert.Equal(BitConverter.ToInt64(this._Bytes), this._Bytes.AsSpan().AsReadOnly().ToInt64());
		Assert.Equal(BitConverter.ToInt64(this._Bytes), this._Bytes.ToInt64());
		Assert.Equal(BitConverter.ToInt64(this._Bytes, 4), this._Bytes.ToInt64(4));
		Assert.Equal(BitConverter.DoubleToInt64Bits(123456789D), 123456789D.ToInt64());
	}

	[Fact]
	public void ToSingle()
	{
		Assert.Equal(BitConverter.ToSingle(this._Bytes), this._Bytes.AsSpan().AsReadOnly().ToSingle());
		Assert.Equal(BitConverter.ToSingle(this._Bytes), this._Bytes.ToSingle());
		Assert.Equal(BitConverter.ToSingle(this._Bytes, 2), this._Bytes.ToSingle(2));
		Assert.Equal(BitConverter.Int32BitsToSingle(123456), 123456.ToSingle());
	}

	[Fact]
	public void ToText()
	{
		Assert.Equal(BitConverter.ToString(this._Bytes), this._Bytes.AsSpan().AsReadOnly().ToText());
		Assert.Equal(BitConverter.ToString(this._Bytes, 2, 4), this._Bytes.AsSpan().AsReadOnly().ToText(2, 4));
		Assert.Equal(BitConverter.ToString(this._Bytes), this._Bytes.ToText());
		Assert.Equal(BitConverter.ToString(this._Bytes, 2), this._Bytes.ToText(2));
		Assert.Equal(BitConverter.ToString(this._Bytes, 2, 4), this._Bytes.ToText(2, 4));
	}

	[Fact]
	public void ToUInt16()
	{
		Assert.Equal(BitConverter.ToInt16(this._Bytes), this._Bytes.AsSpan().AsReadOnly().ToInt16());
		Assert.Equal(BitConverter.ToInt16(this._Bytes), this._Bytes.ToInt16());
		Assert.Equal(BitConverter.ToInt16(this._Bytes, 2), this._Bytes.ToInt16(2));
		Assert.Equal(BitConverter.HalfToInt16Bits((Half)123), ((Half)123).ToInt16());
	}

	[Fact]
	public void ToUInt32()
	{
		Assert.Equal(BitConverter.ToUInt32(this._Bytes), this._Bytes.AsSpan().AsReadOnly().ToUInt32());
		Assert.Equal(BitConverter.ToUInt32(this._Bytes), this._Bytes.ToUInt32());
		Assert.Equal(BitConverter.ToUInt32(this._Bytes, 2), this._Bytes.ToUInt32(2));
		Assert.Equal(BitConverter.SingleToUInt32Bits(123456F), 123456F.ToUInt32());
	}

	[Fact]
	public void ToUInt64()
	{
		Assert.Equal(BitConverter.ToUInt64(this._Bytes), this._Bytes.AsSpan().AsReadOnly().ToUInt64());
		Assert.Equal(BitConverter.ToUInt64(this._Bytes), this._Bytes.ToUInt64());
		Assert.Equal(BitConverter.ToUInt64(this._Bytes, 4), this._Bytes.ToUInt64(4));
		Assert.Equal(BitConverter.DoubleToUInt64Bits(123456789D), 123456789D.ToUInt64());
	}
}
