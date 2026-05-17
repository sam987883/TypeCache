// Copyright (c) 2021 Samuel Abraham

using System.Linq;
using TypeCache.Collections;
using Xunit;

namespace TypeCache.Tests.Collections;

public class BoundLazyDictionaryTests
{
	[Fact]
	public void Create()
	{
		var keys = new[] { "hello", "world" };
		var dictionary = BoundLazyDictionary.Create(keys, key => key.Length);

		Assert.NotNull(dictionary);
		Assert.Equal(2, dictionary.Count);
	}

	[Fact]
	public void Create_WithComparer()
	{
		var keys = new[] { "hello", "world" };
		var dictionary = BoundLazyDictionary.Create(keys, key => key.Length, comparer: StringComparer.OrdinalIgnoreCase);

		Assert.NotNull(dictionary);
		Assert.Equal(2, dictionary.Count);
	}

	[Fact]
	public void Indexer()
	{
		var keys = new[] { "hello", "world" };
		var dictionary = BoundLazyDictionary.Create(keys, key => key.Length);

		Assert.Equal(5, dictionary["hello"]);
		Assert.Equal(5, dictionary["world"]);
	}

	[Fact]
	public void Indexer_NonExistingKey()
	{
		var keys = new[] { "hello", "world" };
		var dictionary = BoundLazyDictionary.Create(keys, key => key.Length, -1);

		Assert.Equal(-1, dictionary["unknown"]);
	}

	[Fact]
	public void ContainsKey()
	{
		var keys = new[] { "hello", "world" };
		var dictionary = BoundLazyDictionary.Create(keys, key => key.Length);

		Assert.True(dictionary.ContainsKey("hello"));
		Assert.False(dictionary.ContainsKey("unknown"));
	}

	[Fact]
	public void Keys()
	{
		var keys = new[] { "hello", "world" };
		var dictionary = BoundLazyDictionary.Create(keys, key => key.Length);

		Assert.Equal(2, dictionary.Keys.Count());
		Assert.Contains("hello", dictionary.Keys);
		Assert.Contains("world", dictionary.Keys);
	}

	[Fact]
	public void Values()
	{
		var keys = new[] { "hello", "world" };
		var dictionary = BoundLazyDictionary.Create(keys, key => key.Length);

		Assert.Equal(2, dictionary.Values.Count());
		Assert.Contains(5, dictionary.Values);
	}

	[Fact]
	public void Count()
	{
		var keys = new[] { "hello", "world" };
		var dictionary = BoundLazyDictionary.Create(keys, key => key.Length);

		Assert.Equal(2, dictionary.Count);
	}

	[Fact]
	public void GetEnumerator()
	{
		var keys = new[] { "hello", "world" };
		var dictionary = BoundLazyDictionary.Create(keys, key => key.Length);

		Assert.Equal(2, dictionary.Count);
	}

	[Fact]
	public void CreateThreadSafe()
	{
		var keys = new[] { "hello", "world" };
		var dictionary = BoundLazyDictionary.CreateThreadSafe(keys, key => key.Length);

		Assert.NotNull(dictionary);
		Assert.Equal(2, dictionary.Count);
	}

	[Fact]
	public void NoValue()
	{
		var keys = new[] { "hello", "world" };
		var dictionary = BoundLazyDictionary.Create(keys, key => key.Length, -1);

		Assert.Equal(-1, dictionary["unknown"]);
	}
}
