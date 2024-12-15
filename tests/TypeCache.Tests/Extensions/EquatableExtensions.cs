// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class EquatableExtensions
{
	[Fact]
	public void ThrowIfEqual()
	{
		"AAA".ThrowIfEqual("AA");
		(null as string).ThrowIfEqual(string.Empty);
		Assert.Throws<ArgumentOutOfRangeException>(() => "bbb".ThrowIfEqual("bbb"));
		Assert.Throws<ArgumentOutOfRangeException>(() => (null as string).ThrowIfEqual(null as string));
		Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".ThrowIfEqual("AAA"));
	}

	[Fact]
	public void ThrowIfNotEqual()
	{
		"AAA".ThrowIfNotEqual("AAA");
		(null as string).ThrowIfNotEqual(null);
		Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".ThrowIfNotEqual("bbb"));
		Assert.Throws<ArgumentOutOfRangeException>(() => (null as string).ThrowIfNotEqual("bbb"));
		Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".ThrowIfNotEqual(null));
	}
}
