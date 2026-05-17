// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;
using Xunit;

namespace TypeCache.Tests.Collections;

public class SetTests
{
	[Fact]
	public void Add()
	{
		var set = new HashSet<int> { 1, 2, 3 };
		var collection = new Set<int>(set);

		var result = ((ISet<int>)collection).Add(4);
		Assert.True(result);
		Assert.True(set.Contains(4));
	}

	[Fact]
	public void ExceptWith()
	{
		var set = new HashSet<int> { 1, 2, 3 };
		var collection = new Set<int>(set);

		collection.ExceptWith(new[] { 2, 3 });
		Assert.Single(set);
		Assert.True(set.Contains(1));
	}

	[Fact]
	public void IntersectWith()
	{
		var set = new HashSet<int> { 1, 2, 3 };
		var collection = new Set<int>(set);

		collection.IntersectWith(new[] { 2, 3, 4 });
		Assert.Equal(2, set.Count);
		Assert.True(set.Contains(2));
		Assert.True(set.Contains(3));
	}

	[Fact]
	public void IsProperSubsetOf()
	{
		var set = new HashSet<int> { 1, 2 };
		var collection = new Set<int>(set);

		Assert.True(collection.IsProperSubsetOf(new[] { 1, 2, 3 }));
		Assert.False(collection.IsProperSubsetOf(new[] { 1, 2 }));
	}

	[Fact]
	public void IsProperSupersetOf()
	{
		var set = new HashSet<int> { 1, 2, 3 };
		var collection = new Set<int>(set);

		Assert.True(collection.IsProperSupersetOf(new[] { 1, 2 }));
		Assert.False(collection.IsProperSupersetOf(new[] { 1, 2, 3 }));
	}

	[Fact]
	public void IsSubsetOf()
	{
		var set = new HashSet<int> { 1, 2 };
		var collection = new Set<int>(set);

		Assert.True(collection.IsSubsetOf(new[] { 1, 2, 3 }));
		Assert.True(collection.IsSubsetOf(new[] { 1, 2 }));
	}

	[Fact]
	public void IsSupersetOf()
	{
		var set = new HashSet<int> { 1, 2, 3 };
		var collection = new Set<int>(set);

		Assert.True(collection.IsSupersetOf(new[] { 1, 2 }));
		Assert.True(collection.IsSupersetOf(new[] { 1, 2, 3 }));
	}

	[Fact]
	public void Overlaps()
	{
		var set = new HashSet<int> { 1, 2, 3 };
		var collection = new Set<int>(set);

		Assert.True(collection.Overlaps(new[] { 2, 3, 4 }));
		Assert.False(collection.Overlaps(new[] { 4, 5, 6 }));
	}

	[Fact]
	public void SetEquals()
	{
		var set = new HashSet<int> { 1, 2, 3 };
		var collection = new Set<int>(set);

		Assert.True(collection.SetEquals(new[] { 1, 2, 3 }));
		Assert.False(collection.SetEquals(new[] { 1, 2 }));
	}

	[Fact]
	public void SymmetricExceptWith()
	{
		var set = new HashSet<int> { 1, 2, 3 };
		var collection = new Set<int>(set);

		collection.SymmetricExceptWith(new[] { 2, 3, 4 });
		Assert.Equal(2, set.Count);
		Assert.True(set.Contains(1));
		Assert.True(set.Contains(4));
	}

	[Fact]
	public void UnionWith()
	{
		var set = new HashSet<int> { 1, 2, 3 };
		var collection = new Set<int>(set);

		collection.UnionWith(new[] { 3, 4, 5 });
		Assert.Equal(5, set.Count);
	}
}
