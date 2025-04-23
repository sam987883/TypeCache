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
	public void Then()
	{
		var actual = false;

		false.Then(() => actual = true);
		Assert.False(actual);

		true.Then(() => actual = true);
		Assert.True(actual);
	}

	[Fact]
	public void ThrowIfFalse()
	{
		true.ThrowIfFalse();
		Assert.Throws<ArgumentOutOfRangeException>(() => false.ThrowIfFalse());
	}

	[Fact]
	public void ThrowIfNotTrue()
	{
		((bool?)true).ThrowIfNotTrue();
		Assert.Throws<ArgumentOutOfRangeException>(() => ((bool?)false).ThrowIfNotTrue());
		Assert.Throws<ArgumentOutOfRangeException>(() => ((bool?)null).ThrowIfNotTrue());
	}

	[Fact]
	public void ThrowIfTrue()
	{
		false.ThrowIfTrue();
		((bool?)false).ThrowIfTrue();
		((bool?)null).ThrowIfTrue();
		Assert.Throws<ArgumentOutOfRangeException>(() => true.ThrowIfTrue());
		Assert.Throws<ArgumentOutOfRangeException>(() => ((bool?)true).ThrowIfTrue());
	}

	[Fact]
	public void ToBytes()
	{
		Assert.Equal(BitConverter.GetBytes(false), false.ToBytes());
		Assert.Equal(BitConverter.GetBytes(true), true.ToBytes());
	}
}
