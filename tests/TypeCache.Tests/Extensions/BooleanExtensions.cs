// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class BooleanExtensions
{
	[Fact]
	public void Else()
	{
		var actual = false;

		true.Else(() => actual = true);
		Assert.False(actual);

		false.Else(() => actual = true);
		Assert.True(actual);
	}

	[Fact]
	public void If()
	{
		var actual = false;

		false.If(() => actual = true);
		Assert.False(actual);

		true.If(() => actual = true);
		Assert.True(actual);

		Assert.False(false.If(true, false));
		Assert.True(true.If(true, false));
	}

	[Fact]
	public void ToBytes()
	{
		Assert.Equal(BitConverter.GetBytes(false), false.ToBytes());
		Assert.Equal(BitConverter.GetBytes(true), true.ToBytes());
	}
}
