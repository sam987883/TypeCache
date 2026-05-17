// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using TypeCache.Collections;
using Xunit;

namespace TypeCache.Tests.Collections;

public class ReadOnlySetTests
{
	[Fact]
	public void Constructor()
	{
		var set = new HashSet<int> { 1, 2, 3 } as IReadOnlySet<int>;
		var readOnlySet = new ReadOnlySet<int>(set);

		Assert.Equal(3, readOnlySet.Count);
	}

	[Fact]
	public void Contains()
	{
		var set = new HashSet<int> { 1, 2, 3 } as IReadOnlySet<int>;
		var readOnlySet = new ReadOnlySet<int>(set);

		Assert.True(readOnlySet.Contains(1));
		Assert.False(readOnlySet.Contains(5));
	}

	[Fact]
	public void IsProperSubsetOf()
	{
		var set1 = new HashSet<int> { 1, 2 } as IReadOnlySet<int>;
		var readOnlySet1 = new ReadOnlySet<int>(set1);

		var set2 = new[] { 1, 2, 3 };

		Assert.True(readOnlySet1.IsProperSubsetOf(set2));
	}

	[Fact]
	public void IsProperSupersetOf()
	{
		var set1 = new HashSet<int> { 1, 2, 3 } as IReadOnlySet<int>;
		var readOnlySet1 = new ReadOnlySet<int>(set1);

		var set2 = new[] { 1, 2 };

		Assert.True(readOnlySet1.IsProperSupersetOf(set2));
	}

	[Fact]
	public void IsSubsetOf()
	{
		var set1 = new HashSet<int> { 1, 2 } as IReadOnlySet<int>;
		var readOnlySet1 = new ReadOnlySet<int>(set1);

		var set2 = new[] { 1, 2, 3 };

		Assert.True(readOnlySet1.IsSubsetOf(set2));
	}

	[Fact]
	public void IsSupersetOf()
	{
		var set1 = new HashSet<int> { 1, 2, 3 } as IReadOnlySet<int>;
		var readOnlySet1 = new ReadOnlySet<int>(set1);

		var set2 = new[] { 1, 2 };

		Assert.True(readOnlySet1.IsSupersetOf(set2));
	}

	[Fact]
	public void Overlaps()
	{
		var set1 = new HashSet<int> { 1, 2, 3 } as IReadOnlySet<int>;
		var readOnlySet1 = new ReadOnlySet<int>(set1);

		var set2 = new[] { 2, 3, 4 };

		Assert.True(readOnlySet1.Overlaps(set2));
	}

	[Fact]
	public void SetEquals()
	{
		var set1 = new HashSet<int> { 1, 2, 3 } as IReadOnlySet<int>;
		var readOnlySet1 = new ReadOnlySet<int>(set1);

		var set2 = new[] { 1, 2, 3 };

		Assert.True(readOnlySet1.SetEquals(set2));
	}
}
