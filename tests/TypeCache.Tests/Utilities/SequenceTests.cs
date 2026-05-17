// Copyright (c) 2021 Samuel Abraham

using System.Linq;
using TypeCache.Utilities;
using Xunit;

namespace TypeCache.Tests.Utilities;

public class SequenceTests
{
	[Fact]
	public void ExponentialSeconds_Count()
	{
		var sequence = Sequence.ExponentialSeconds(5);

		Assert.Equal(5, sequence.Count());
	}

	[Fact]
	public void ExponentialSeconds_IncreasingValues()
	{
		var sequence = Sequence.ExponentialSeconds(3).ToList();

		Assert.True(sequence[0] < sequence[1]);
		Assert.True(sequence[1] < sequence[2]);
	}

	[Fact]
	public void ExponentialSeconds_WithExponent()
	{
		var sequence = Sequence.ExponentialSeconds(exponent: 2, count: 3).ToList();

		Assert.Equal(3, sequence.Count);
		Assert.True(sequence[0] < sequence[1]);
		Assert.True(sequence[1] < sequence[2]);
	}

	[Fact]
	public void LinearTime_Count()
	{
		var sequence = Sequence.LinearTime(TimeSpan.FromSeconds(1), 5);

		Assert.Equal(5, sequence.Count());
	}

	[Fact]
	public void LinearTime_IncreasingByFixed()
	{
		var increase = TimeSpan.FromSeconds(5);
		var sequence = Sequence.LinearTime(increase, 3).ToList();

		Assert.Equal(increase, sequence[0]);
		Assert.Equal(increase * 2, sequence[1]);
		Assert.Equal(increase * 3, sequence[2]);
	}
}
