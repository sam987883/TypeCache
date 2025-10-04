// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using TypeCache.Adapters;
using TypeCache.Collections;
using Xunit;

namespace TypeCache.Tests.Adapters;

public class CollectionAdapterTests
{
	[Fact]
	public void CountTest()
	{
		var items = new[] { 1, 2, 3, 4, 5, 6 };
		var adapter = new CollectionAdapter(items);

		Assert.Equal(items.Length, adapter.Count);
	}

	[Fact]
	public void IsReadOnlyTest()
	{
		var items = new[] { 1, 2, 3, 4, 5, 6 };
		var adapter = new CollectionAdapter(items);

		Assert.True(adapter.IsReadOnly);
	}

	[Fact]
	public void AddTest()
	{
		var items = new List<int>(7) { 1, 2, 3, 4, 5, 6 };
		var adapter = new CollectionAdapter(items);
		adapter.Add(7);

		Assert.Equal(items.Count, adapter.Count);
	}

	[Fact]
	public void ClearTest()
	{
		var items = new List<int>(7) { 1, 2, 3, 4, 5, 6 };
		var adapter = new CollectionAdapter(items);
		adapter.Clear();

		Assert.Equal(items.Count, adapter.Count);
	}

	[Fact]
	public void ContainsTest()
	{
		var items = new List<int>(7) { 1, 2, 3, 4, 5, 6 };
		var adapter = new CollectionAdapter(items);

		Assert.True(adapter.Contains(3));
	}

	[Fact]
	public void CopyToTest()
	{
		var items = new[] { 1, 2, 3, 4, 5, 6 };
		var other = new object[6];
		var adapter = new CollectionAdapter(items);
		adapter.CopyTo(other, 0);

		Assert.Equivalent(items, other);
	}

	[Fact]
	public void GetEnumeratorTest()
	{
		var items = new List<int>(7) { 1, 2, 3, 4, 5, 6 };
		var adapter = new CollectionAdapter(items);

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

	[Fact]
	public void RemoveTest()
	{
		var items = new List<int>(7) { 1, 2, 3, 4, 5, 6 };
		var adapter = new CollectionAdapter(items);
		adapter.Remove(5);

		Assert.Equal(items.Count, adapter.Count);
	}
}
