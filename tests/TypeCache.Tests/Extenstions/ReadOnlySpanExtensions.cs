// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extenstions;

public class ReadOnlySpanExtensions
{
	[Fact]
	public void ToBoolean()
	{
		Assert.True(((ReadOnlySpan<byte>)true.ToBytes().AsSpan()).To<bool>());
		Assert.False(((ReadOnlySpan<byte>)false.ToBytes().AsSpan()).To<bool>());
	}

	[Fact]
	public void ToChar()
	{
		Assert.Equal(char.MinValue, ((ReadOnlySpan<byte>)char.MinValue.ToBytes()).To<char>());
		Assert.Equal(char.MaxValue, ((ReadOnlySpan<byte>)char.MaxValue.ToBytes()).To<char>());
	}

	[Fact]
	public void ToDouble()
	{
		Assert.Equal(double.MinValue, ((ReadOnlySpan<byte>)double.MinValue.ToBytes()).To<double>());
		Assert.Equal(double.MaxValue, ((ReadOnlySpan<byte>)double.MaxValue.ToBytes()).To<double>());
	}

	[Fact]
	public void ToInt16()
	{
		Assert.Equal(short.MinValue, ((ReadOnlySpan<byte>)short.MinValue.ToBytes()).To<short>());
		Assert.Equal(short.MaxValue, ((ReadOnlySpan<byte>)short.MaxValue.ToBytes()).To<short>());
	}

	[Fact]
	public void ToInt32()
	{
		Assert.Equal(int.MinValue, ((ReadOnlySpan<byte>)int.MinValue.ToBytes()).To<int>());
		Assert.Equal(int.MaxValue, ((ReadOnlySpan<byte>)int.MaxValue.ToBytes()).To<int>());
	}

	[Fact]
	public void ToInt64()
	{
		Assert.Equal(long.MinValue, ((ReadOnlySpan<byte>)long.MinValue.ToBytes()).To<long>());
		Assert.Equal(long.MaxValue, ((ReadOnlySpan<byte>)long.MaxValue.ToBytes()).To<long>());
	}

	[Fact]
	public void ToSingle()
	{
		Assert.Equal(float.MinValue, ((ReadOnlySpan<byte>)float.MinValue.ToBytes()).To<float>());
		Assert.Equal(float.MaxValue, ((ReadOnlySpan<byte>)float.MaxValue.ToBytes()).To<float>());
	}

	[Fact]
	public void ToUInt16()
	{
		Assert.Equal(ushort.MinValue, ((ReadOnlySpan<byte>)ushort.MinValue.ToBytes()).To<ushort>());
		Assert.Equal(ushort.MaxValue, ((ReadOnlySpan<byte>)ushort.MaxValue.ToBytes()).To<ushort>());
	}

	[Fact]
	public void ToUInt32()
	{
		Assert.Equal(uint.MinValue, ((ReadOnlySpan<byte>)uint.MinValue.ToBytes()).To<uint>());
		Assert.Equal(uint.MaxValue, ((ReadOnlySpan<byte>)uint.MaxValue.ToBytes()).To<uint>());
	}

	[Fact]
	public void ToUInt64()
	{
		Assert.Equal(ulong.MinValue, ((ReadOnlySpan<byte>)ulong.MinValue.ToBytes()).To<ulong>());
		Assert.Equal(ulong.MaxValue, ((ReadOnlySpan<byte>)ulong.MaxValue.ToBytes()).To<ulong>());
	}
}
