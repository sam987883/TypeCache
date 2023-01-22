// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class ReadOnlySpanExtensions
{
	[Fact]
	public void ToBoolean()
	{
		Assert.True(((ReadOnlySpan<byte>)true.GetBytes().AsSpan()).To<bool>());
		Assert.False(((ReadOnlySpan<byte>)false.GetBytes().AsSpan()).To<bool>());
	}

	[Fact]
	public void ToChar()
	{
		Assert.Equal(char.MinValue, ((ReadOnlySpan<byte>)char.MinValue.GetBytes()).To<char>());
		Assert.Equal(char.MaxValue, ((ReadOnlySpan<byte>)char.MaxValue.GetBytes()).To<char>());
	}

	[Fact]
	public void ToDouble()
	{
		Assert.Equal(double.MinValue, ((ReadOnlySpan<byte>)double.MinValue.GetBytes()).To<double>());
		Assert.Equal(double.MaxValue, ((ReadOnlySpan<byte>)double.MaxValue.GetBytes()).To<double>());
	}

	[Fact]
	public void ToInt16()
	{
		Assert.Equal(short.MinValue, ((ReadOnlySpan<byte>)short.MinValue.GetBytes()).To<short>());
		Assert.Equal(short.MaxValue, ((ReadOnlySpan<byte>)short.MaxValue.GetBytes()).To<short>());
	}

	[Fact]
	public void ToInt32()
	{
		Assert.Equal(int.MinValue, ((ReadOnlySpan<byte>)int.MinValue.GetBytes()).To<int>());
		Assert.Equal(int.MaxValue, ((ReadOnlySpan<byte>)int.MaxValue.GetBytes()).To<int>());
	}

	[Fact]
	public void ToInt64()
	{
		Assert.Equal(long.MinValue, ((ReadOnlySpan<byte>)long.MinValue.GetBytes()).To<long>());
		Assert.Equal(long.MaxValue, ((ReadOnlySpan<byte>)long.MaxValue.GetBytes()).To<long>());
	}

	[Fact]
	public void ToSingle()
	{
		Assert.Equal(float.MinValue, ((ReadOnlySpan<byte>)float.MinValue.GetBytes()).To<float>());
		Assert.Equal(float.MaxValue, ((ReadOnlySpan<byte>)float.MaxValue.GetBytes()).To<float>());
	}

	[Fact]
	public void ToUInt16()
	{
		Assert.Equal(ushort.MinValue, ((ReadOnlySpan<byte>)ushort.MinValue.GetBytes()).To<ushort>());
		Assert.Equal(ushort.MaxValue, ((ReadOnlySpan<byte>)ushort.MaxValue.GetBytes()).To<ushort>());
	}

	[Fact]
	public void ToUInt32()
	{
		Assert.Equal(uint.MinValue, ((ReadOnlySpan<byte>)uint.MinValue.GetBytes()).To<uint>());
		Assert.Equal(uint.MaxValue, ((ReadOnlySpan<byte>)uint.MaxValue.GetBytes()).To<uint>());
	}

	[Fact]
	public void ToUInt64()
	{
		Assert.Equal(ulong.MinValue, ((ReadOnlySpan<byte>)ulong.MinValue.GetBytes()).To<ulong>());
		Assert.Equal(ulong.MaxValue, ((ReadOnlySpan<byte>)ulong.MaxValue.GetBytes()).To<ulong>());
	}
}
