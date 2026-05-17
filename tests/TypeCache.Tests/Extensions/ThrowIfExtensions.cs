using System;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class ThrowIfExtensions
{

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
}
