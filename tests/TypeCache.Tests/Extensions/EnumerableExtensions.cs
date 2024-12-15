// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class EnumerableExtensions
{
	[Fact]
	public void ThrowIfEmpty()
	{
		"AAA".ThrowIfEmpty();
		Assert.Throws<ArgumentNullException>(() => (null as string).ThrowIfEmpty());
		Assert.Throws<ArgumentOutOfRangeException>(() => string.Empty.ThrowIfEmpty());
	}
}
