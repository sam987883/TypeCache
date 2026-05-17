// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;
using Xunit;

namespace TypeCache.Tests.Collections;

public class CollectionTests
{
	[Fact]
	public void Add()
	{
		var list = new List<int> { 1, 2, 3 };
		var collection = new Collection<int>(list);

		collection.Add(4);
		Assert.True(list.Contains(4));
	}

	[Fact]
	public void Clear()
	{
		var list = new List<int> { 1, 2, 3 };
		var collection = new Collection<int>(list);

		collection.Clear();
		Assert.Empty(list);
	}

	[Fact]
	public void Contains()
	{
		var list = new List<int> { 1, 2, 3 };
		var collection = new Collection<int>(list);

		Assert.True(collection.Contains(2));
		Assert.False(collection.Contains(5));
	}

	[Fact]
	public void CopyTo()
	{
		var list = new List<int> { 1, 2, 3 };
		var collection = new Collection<int>(list);
		var array = new int[3];

		collection.CopyTo(array, 0);
		Assert.Equal([1, 2, 3], array);
	}

	[Fact]
	public void Count()
	{
		var list = new List<int> { 1, 2, 3 };
		var collection = new Collection<int>(list);

		Assert.Equal(3, collection.Count);
	}

	[Fact]
	public void IsReadOnly()
	{
		var list = new List<int> { 1, 2, 3 };
		var collection = new Collection<int>(list);

		Assert.False(collection.IsReadOnly);
	}

	[Fact]
	public void GetEnumerator()
	{
		var list = new List<int> { 1, 2, 3 };
		var collection = new Collection<int>(list);

		var items = new List<int>();
		foreach (var item in collection)
			items.Add(item);

		Assert.Equal([1, 2, 3], items);
	}

	[Fact]
	public void Remove()
	{
		var list = new List<int> { 1, 2, 3 };
		var collection = new Collection<int>(list);

		var removed = collection.Remove(2);
		Assert.True(removed);
		Assert.False(list.Contains(2));
	}
}
