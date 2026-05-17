// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Linq;
using TypeCache.Collections;
using Xunit;

namespace TypeCache.Tests.Collections;

public class ReadOnlyListTests
{
	[Fact]
	public void Constructor()
	{
		var list = new[] { 1, 2, 3 } as IReadOnlyList<int>;
		var readOnlyList = new ReadOnlyList<int>(list);

		Assert.Equal(3, readOnlyList.Count);
	}

	[Fact]
	public void Indexer()
	{
		var list = new[] { 1, 2, 3 } as IReadOnlyList<int>;
		var readOnlyList = new ReadOnlyList<int>(list);

		Assert.Equal(1, readOnlyList[0]);
		Assert.Equal(2, readOnlyList[1]);
		Assert.Equal(3, readOnlyList[2]);
	}

	[Fact]
	public void Count()
	{
		var list = new[] { 1, 2, 3 } as IReadOnlyList<int>;
		var readOnlyList = new ReadOnlyList<int>(list);

		Assert.Equal(3, readOnlyList.Count);
	}

	[Fact]
	public void GetEnumerator()
	{
		var list = new[] { 1, 2, 3 } as IReadOnlyList<int>;
		var readOnlyList = new ReadOnlyList<int>(list);

		var items = readOnlyList.ToList();
		Assert.Equal([1, 2, 3], items);
	}
}
