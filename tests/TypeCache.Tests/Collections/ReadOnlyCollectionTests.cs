// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;
using Xunit;

namespace TypeCache.Tests.Collections;

public class ReadOnlyCollectionTests
{
	[Fact]
	public void Constructor_WithIEnumerable()
	{
		var list = new[] { 1, 2, 3 };
		var collection = new ReadOnlyCollection<int>(list);

		Assert.Equal(3, collection.Count);
	}

	[Fact]
	public void Constructor_WithIReadOnlyCollection()
	{
		var list = new[] { 1, 2, 3 } as IReadOnlyCollection<int>;
		var collection = new ReadOnlyCollection<int>(list);

		Assert.Equal(3, collection.Count);
	}

	[Fact]
	public void Count()
	{
		var list = new[] { 1, 2, 3 };
		var collection = new ReadOnlyCollection<int>(list);

		Assert.Equal(3, collection.Count);
	}

	[Fact]
	public void GetEnumerator()
	{
		var list = new[] { 1, 2, 3 };
		var collection = new ReadOnlyCollection<int>(list);

		Assert.Equal([1, 2, 3], collection);
	}

	[Fact]
	public void GetEnumerator_Empty()
	{
		var list = Array.Empty<int>();
		var collection = new ReadOnlyCollection<int>(list);

		Assert.Empty(collection);
	}

	[Fact]
	public void GetEnumerator_Multiple()
	{
		var list = new[] { 1, 2, 3 };
		var collection = new ReadOnlyCollection<int>(list);

		var enumerator1 = collection.GetEnumerator();
		var enumerator2 = collection.GetEnumerator();

		Assert.NotSame(enumerator1, enumerator2);
	}
}
