// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class ReadOnlySpanExtensions
{
	[Fact]
	public void Read()
	{
		Assert.True(((ReadOnlySpan<byte>)true.ToBytes().AsSpan()).Read<bool>());
		Assert.False(((ReadOnlySpan<byte>)false.ToBytes().AsSpan()).Read<bool>());
		Assert.Equal(char.MinValue, ((ReadOnlySpan<byte>)char.MinValue.ToBytes()).Read<char>());
		Assert.Equal(char.MaxValue, ((ReadOnlySpan<byte>)char.MaxValue.ToBytes()).Read<char>());
		Assert.Equal(float.MinValue, ((ReadOnlySpan<byte>)float.MinValue.ToBytes()).Read<float>());
		Assert.Equal(float.MaxValue, ((ReadOnlySpan<byte>)float.MaxValue.ToBytes()).Read<float>());
		Assert.Equal(double.MinValue, ((ReadOnlySpan<byte>)double.MinValue.ToBytes()).Read<double>());
		Assert.Equal(double.MaxValue, ((ReadOnlySpan<byte>)double.MaxValue.ToBytes()).Read<double>());
		Assert.Equal(short.MinValue, ((ReadOnlySpan<byte>)short.MinValue.ToBytes()).Read<short>());
		Assert.Equal(short.MaxValue, ((ReadOnlySpan<byte>)short.MaxValue.ToBytes()).Read<short>());
		Assert.Equal(int.MinValue, ((ReadOnlySpan<byte>)int.MinValue.ToBytes()).Read<int>());
		Assert.Equal(int.MaxValue, ((ReadOnlySpan<byte>)int.MaxValue.ToBytes()).Read<int>());
		Assert.Equal(long.MinValue, ((ReadOnlySpan<byte>)long.MinValue.ToBytes()).Read<long>());
		Assert.Equal(long.MaxValue, ((ReadOnlySpan<byte>)long.MaxValue.ToBytes()).Read<long>());
		Assert.Equal(ushort.MinValue, ((ReadOnlySpan<byte>)ushort.MinValue.ToBytes()).Read<ushort>());
		Assert.Equal(ushort.MaxValue, ((ReadOnlySpan<byte>)ushort.MaxValue.ToBytes()).Read<ushort>());
		Assert.Equal(uint.MinValue, ((ReadOnlySpan<byte>)uint.MinValue.ToBytes()).Read<uint>());
		Assert.Equal(uint.MaxValue, ((ReadOnlySpan<byte>)uint.MaxValue.ToBytes()).Read<uint>());
		Assert.Equal(ulong.MinValue, ((ReadOnlySpan<byte>)ulong.MinValue.ToBytes()).Read<ulong>());
		Assert.Equal(ulong.MaxValue, ((ReadOnlySpan<byte>)ulong.MaxValue.ToBytes()).Read<ulong>());
	}
}
