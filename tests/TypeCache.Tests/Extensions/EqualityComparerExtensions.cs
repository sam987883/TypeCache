// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class EqualityComparerExtensions
{
	[Fact]
	public void ThrowIfEqual()
	{
		StringComparer.Ordinal.ThrowIfEqual("AAA", "BBB");
		StringComparer.OrdinalIgnoreCase.ThrowIfEqual("AAA", "bbb");
		StringComparer.OrdinalIgnoreCase.ThrowIfEqual("AAA", "aba");
		Assert.Throws<ArgumentOutOfRangeException>(() => StringComparer.Ordinal.ThrowIfEqual("AAA", "AAA"));
		Assert.Throws<ArgumentOutOfRangeException>(() => StringComparer.OrdinalIgnoreCase.ThrowIfEqual("AAA", "aaa"));
		Assert.Throws<ArgumentOutOfRangeException>(() => StringComparer.OrdinalIgnoreCase.ThrowIfEqual("ABA", "aba"));
	}

	[Fact]
	public void ThrowIfNotEqual()
	{
		StringComparer.Ordinal.ThrowIfNotEqual("AAA", "AAA");
		StringComparer.OrdinalIgnoreCase.ThrowIfNotEqual("AAA", "aaa");
		StringComparer.OrdinalIgnoreCase.ThrowIfNotEqual("AAA", "AAA");
		Assert.Throws<ArgumentOutOfRangeException>(() => StringComparer.Ordinal.ThrowIfNotEqual("AAA", "bbb"));
		Assert.Throws<ArgumentOutOfRangeException>(() => StringComparer.OrdinalIgnoreCase.ThrowIfNotEqual("AAA", "bbb"));
	}
}
