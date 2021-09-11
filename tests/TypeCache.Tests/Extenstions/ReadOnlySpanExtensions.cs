// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extenstions
{
	public class ReadOnlySpanExtensions
	{
		[Fact]
		public void ToBoolean()
		{
			Assert.True(((ReadOnlySpan<byte>)true.ToBytes().AsSpan()).ToBoolean());
			Assert.False(((ReadOnlySpan<byte>)false.ToBytes().AsSpan()).ToBoolean());
		}

		[Fact]
		public void ToChar()
		{
			Assert.Equal(char.MinValue, ((ReadOnlySpan<byte>)char.MinValue.ToBytes()).ToChar());
			Assert.Equal(char.MaxValue, ((ReadOnlySpan<byte>)char.MaxValue.ToBytes()).ToChar());
		}

		[Fact]
		public void ToDouble()
		{
			Assert.Equal(double.MinValue, ((ReadOnlySpan<byte>)double.MinValue.ToBytes()).ToDouble());
			Assert.Equal(double.MaxValue, ((ReadOnlySpan<byte>)double.MaxValue.ToBytes()).ToDouble());
		}

		[Fact]
		public void ToInt16()
		{
			Assert.Equal(short.MinValue, ((ReadOnlySpan<byte>)short.MinValue.ToBytes()).ToInt16());
			Assert.Equal(short.MaxValue, ((ReadOnlySpan<byte>)short.MaxValue.ToBytes()).ToInt16());
		}

		[Fact]
		public void ToInt32()
		{
			Assert.Equal(int.MinValue, ((ReadOnlySpan<byte>)int.MinValue.ToBytes()).ToInt32());
			Assert.Equal(int.MaxValue, ((ReadOnlySpan<byte>)int.MaxValue.ToBytes()).ToInt32());
		}

		[Fact]
		public void ToInt64()
		{
			Assert.Equal(long.MinValue, ((ReadOnlySpan<byte>)long.MinValue.ToBytes()).ToInt64());
			Assert.Equal(long.MaxValue, ((ReadOnlySpan<byte>)long.MaxValue.ToBytes()).ToInt64());
		}

		[Fact]
		public void ToSingle()
		{
			Assert.Equal(float.MinValue, ((ReadOnlySpan<byte>)float.MinValue.ToBytes()).ToSingle());
			Assert.Equal(float.MaxValue, ((ReadOnlySpan<byte>)float.MaxValue.ToBytes()).ToSingle());
		}

		[Fact]
		public void ToUInt16()
		{
			Assert.Equal(ushort.MinValue, ((ReadOnlySpan<byte>)ushort.MinValue.ToBytes()).ToUInt16());
			Assert.Equal(ushort.MaxValue, ((ReadOnlySpan<byte>)ushort.MaxValue.ToBytes()).ToUInt16());
		}

		[Fact]
		public void ToUInt32()
		{
			Assert.Equal(uint.MinValue, ((ReadOnlySpan<byte>)uint.MinValue.ToBytes()).ToUInt32());
			Assert.Equal(uint.MaxValue, ((ReadOnlySpan<byte>)uint.MaxValue.ToBytes()).ToUInt32());
		}

		[Fact]
		public void ToUInt64()
		{
			Assert.Equal(ulong.MinValue, ((ReadOnlySpan<byte>)ulong.MinValue.ToBytes()).ToUInt64());
			Assert.Equal(ulong.MaxValue, ((ReadOnlySpan<byte>)ulong.MaxValue.ToBytes()).ToUInt64());
		}
	}
}
