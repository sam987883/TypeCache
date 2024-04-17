// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class IndexExtensions
{
	[Fact]
	public void FromStart()
	{
		Assert.Equal(2, new Index(8, true).FromStart(10).Value);
		Assert.Equal(6, new Index(6).FromStart(10).Value);
	}

	[Fact]
	public void Next()
	{
		Assert.Equal(5, new Index(3, true).Next(2).Value);
		Assert.Equal(5, new Index(4, true).Next().Value);
	}

	[Fact]
	public void Previous()
	{
		Assert.Equal(3, new Index(5, true).Previous(2).Value);
		Assert.Equal(3, new Index(4, true).Previous().Value);
	}
}
