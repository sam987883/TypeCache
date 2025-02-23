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
		Assert.True(((ReadOnlySpan<byte>)true.GetBytes().AsSpan()).Read<bool>());
		Assert.False(((ReadOnlySpan<byte>)false.GetBytes().AsSpan()).Read<bool>());
		Assert.Equal(char.MinValue, ((ReadOnlySpan<byte>)char.MinValue.GetBytes()).Read<char>());
		Assert.Equal(char.MaxValue, ((ReadOnlySpan<byte>)char.MaxValue.GetBytes()).Read<char>());
		Assert.Equal(float.MinValue, ((ReadOnlySpan<byte>)float.MinValue.GetBytes()).Read<float>());
		Assert.Equal(float.MaxValue, ((ReadOnlySpan<byte>)float.MaxValue.GetBytes()).Read<float>());
		Assert.Equal(double.MinValue, ((ReadOnlySpan<byte>)double.MinValue.GetBytes()).Read<double>());
		Assert.Equal(double.MaxValue, ((ReadOnlySpan<byte>)double.MaxValue.GetBytes()).Read<double>());
		Assert.Equal(short.MinValue, ((ReadOnlySpan<byte>)short.MinValue.GetBytes()).Read<short>());
		Assert.Equal(short.MaxValue, ((ReadOnlySpan<byte>)short.MaxValue.GetBytes()).Read<short>());
		Assert.Equal(int.MinValue, ((ReadOnlySpan<byte>)int.MinValue.GetBytes()).Read<int>());
		Assert.Equal(int.MaxValue, ((ReadOnlySpan<byte>)int.MaxValue.GetBytes()).Read<int>());
		Assert.Equal(long.MinValue, ((ReadOnlySpan<byte>)long.MinValue.GetBytes()).Read<long>());
		Assert.Equal(long.MaxValue, ((ReadOnlySpan<byte>)long.MaxValue.GetBytes()).Read<long>());
		Assert.Equal(ushort.MinValue, ((ReadOnlySpan<byte>)ushort.MinValue.GetBytes()).Read<ushort>());
		Assert.Equal(ushort.MaxValue, ((ReadOnlySpan<byte>)ushort.MaxValue.GetBytes()).Read<ushort>());
		Assert.Equal(uint.MinValue, ((ReadOnlySpan<byte>)uint.MinValue.GetBytes()).Read<uint>());
		Assert.Equal(uint.MaxValue, ((ReadOnlySpan<byte>)uint.MaxValue.GetBytes()).Read<uint>());
		Assert.Equal(ulong.MinValue, ((ReadOnlySpan<byte>)ulong.MinValue.GetBytes()).Read<ulong>());
		Assert.Equal(ulong.MaxValue, ((ReadOnlySpan<byte>)ulong.MaxValue.GetBytes()).Read<ulong>());
	}
}
