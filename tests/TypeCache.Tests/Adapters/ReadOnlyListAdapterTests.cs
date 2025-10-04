// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using TypeCache.Adapters;
using Xunit;

namespace TypeCache.Tests.Adapters;

public class ReadOnlyListAdapterTests
{
	[Fact]
	public void CountTest()
	{
		var items = new[] { 1, 2, 3, 4, 5, 6 };
		var adapter = new ReadOnlyListAdapter(items);

		Assert.Equal(items.Length, adapter.Count);
	}

	[Fact]
	public void DefaultIndexerTest()
	{
		var items = new[] { 1, 2, 3, 4, 5, 6 };
		var adapter = new ReadOnlyListAdapter(items);

		Assert.Equal(4, adapter[3]);
	}

	[Fact]
	public void GetEnumeratorTest()
	{
		var items = new List<int>(7) { 1, 2, 3, 4, 5, 6 };
		var adapter = new ReadOnlyListAdapter(items);

		var i = 0;
		var enumerator = adapter.GetEnumerator();
		while (enumerator.MoveNext())
			++i;

		Assert.Equal(6, i);

		i = 0;
		var enumerator2 = ((IEnumerable<object>)adapter).GetEnumerator();
		while (enumerator2.MoveNext())
			++i;

		Assert.Equal(6, i);
	}
}
