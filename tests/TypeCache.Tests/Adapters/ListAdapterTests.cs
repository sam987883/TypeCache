// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using TypeCache.Adapters;
using Xunit;

namespace TypeCache.Tests.Adapters;

public class ListAdapterTests
{
	[Fact]
	public void CountTest()
	{
		var items = new[] { 1, 2, 3, 4, 5, 6 };
		var adapter = new ListAdapter(items);

		Assert.Equal(items.Length, adapter.Count);
	}

	[Fact]
	public void IsReadOnlyTest()
	{
		var items = new[] { 1, 2, 3, 4, 5, 6 };
		var adapter = new ListAdapter(items);

		Assert.True(adapter.IsReadOnly);
	}

	[Fact]
	public void DefaultIndexerTest()
	{
		var items = new[] { 1, 2, 3, 4, 5, 6 };
		var adapter = new ListAdapter(items);

		adapter[3] = 999999;
		Assert.Equal(999999, adapter[3]);
	}

	[Fact]
	public void AddTest()
	{
		var items = new List<int>(7) { 1, 2, 3, 4, 5, 6 };
		var adapter = new ListAdapter(items);
		adapter.Add(7);

		Assert.Equal(items.Count, adapter.Count);
	}

	[Fact]
	public void ClearTest()
	{
		var items = new List<int>(7) { 1, 2, 3, 4, 5, 6 };
		var adapter = new ListAdapter(items);
		adapter.Clear();

		Assert.Equal(items.Count, adapter.Count);
	}

	[Fact]
	public void ContainsTest()
	{
		var items = new List<int>(7) { 1, 2, 3, 4, 5, 6 };
		var adapter = new ListAdapter(items);

		Assert.True(adapter.Contains(3));
	}

	[Fact]
	public void CopyToTest()
	{
		var items = new[] { 1, 2, 3, 4, 5, 6 };
		var other = new object[6];
		var adapter = new ListAdapter(items);
		adapter.CopyTo(other, 0);

		Assert.Equivalent(items, other);
	}

	[Fact]
	public void GetEnumeratorTest()
	{
		var items = new List<int>(7) { 1, 2, 3, 4, 5, 6 };
		var adapter = new ListAdapter(items);

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
	public void IndexOfTest()
	{
		var items = new[] { 1, 2, 3, 4, 5, 6 };
		var adapter = new ListAdapter(items);

		Assert.Equal(0, adapter.IndexOf(1));
		Assert.Equal(3, adapter.IndexOf(4));
		Assert.Equal(5, adapter.IndexOf(6));
	}

	[Fact]
	public void InsertTest()
	{
		var items = new List<int>(7) { 1, 2, 3, 4, 5, 6 };
		var adapter = new ListAdapter(items);
		adapter.Insert(2, 999);

		Assert.Equal(items.Count, adapter.Count);
		Assert.Equal(999, adapter[2]);
	}

	[Fact]
	public void RemoveTest()
	{
		var items = new List<int>(7) { 1, 2, 3, 4, 5, 6 };
		var adapter = new ListAdapter(items);
		adapter.Remove(5);

		Assert.Equal(items.Count, adapter.Count);
	}

	[Fact]
	public void RemoveAtTest()
	{
		var items = new List<int>(7) { 1, 2, 3, 4, 5, 6 };
		var adapter = new ListAdapter(items);
		adapter.RemoveAt(5);

		Assert.Equal(items.Count, adapter.Count);
	}
}
