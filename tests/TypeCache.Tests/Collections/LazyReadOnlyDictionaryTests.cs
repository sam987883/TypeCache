// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;
using Xunit;

namespace TypeCache.Tests.Collections;

public class LazyReadOnlyDictionaryTests
{
	[Fact]
	public void Constructor()
	{
		var lazyDictionary = new Dictionary<string, Lazy<int>>
		{
			{ "hello", new Lazy<int>(() => 5) },
			{ "world", new Lazy<int>(() => 5) }
		} as IReadOnlyDictionary<string, Lazy<int>>;

		var dictionary = new LazyReadOnlyDictionary<string, int>(lazyDictionary);

		Assert.NotNull(dictionary);
		Assert.Equal(2, dictionary.Count);
	}

	[Fact]
	public void Indexer()
	{
		var lazyDictionary = new Dictionary<string, Lazy<int>>
		{
			{ "hello", new Lazy<int>(() => 5) },
			{ "world", new Lazy<int>(() => 5) }
		} as IReadOnlyDictionary<string, Lazy<int>>;

		var dictionary = new LazyReadOnlyDictionary<string, int>(lazyDictionary);

		Assert.Equal(5, dictionary["hello"]);
		Assert.Equal(5, dictionary["world"]);
	}

	[Fact]
	public void ContainsKey()
	{
		var lazyDictionary = new Dictionary<string, Lazy<int>>
		{
			{ "hello", new Lazy<int>(() => 5) },
			{ "world", new Lazy<int>(() => 5) }
		} as IReadOnlyDictionary<string, Lazy<int>>;

		var dictionary = new LazyReadOnlyDictionary<string, int>(lazyDictionary);

		Assert.True(dictionary.ContainsKey("hello"));
		Assert.False(dictionary.ContainsKey("unknown"));
	}

	[Fact]
	public void Keys()
	{
		var lazyDictionary = new Dictionary<string, Lazy<int>>
		{
			{ "hello", new Lazy<int>(() => 5) },
			{ "world", new Lazy<int>(() => 5) }
		} as IReadOnlyDictionary<string, Lazy<int>>;

		var dictionary = new LazyReadOnlyDictionary<string, int>(lazyDictionary);

		Assert.Equal(2, dictionary.Count);
		Assert.Contains("hello", dictionary.Keys);
		Assert.Contains("world", dictionary.Keys);
	}

	[Fact]
	public void Values()
	{
		var lazyDictionary = new Dictionary<string, Lazy<int>>
		{
			{ "hello", new Lazy<int>(() => 5) },
			{ "world", new Lazy<int>(() => 5) }
		} as IReadOnlyDictionary<string, Lazy<int>>;

		var dictionary = new LazyReadOnlyDictionary<string, int>(lazyDictionary);

		Assert.Equal(2, dictionary.Count);
		Assert.Contains(5, dictionary.Values);
	}

	[Fact]
	public void Count()
	{
		var lazyDictionary = new Dictionary<string, Lazy<int>>
		{
			{ "hello", new Lazy<int>(() => 5) },
			{ "world", new Lazy<int>(() => 5) }
		} as IReadOnlyDictionary<string, Lazy<int>>;

		var dictionary = new LazyReadOnlyDictionary<string, int>(lazyDictionary);

		Assert.Equal(2, dictionary.Count);
	}

	[Fact]
	public void GetEnumerator()
	{
		var lazyDictionary = new Dictionary<string, Lazy<int>>
		{
			{ "hello", new Lazy<int>(() => 5) },
			{ "world", new Lazy<int>(() => 5) }
		} as IReadOnlyDictionary<string, Lazy<int>>;

		var dictionary = new LazyReadOnlyDictionary<string, int>(lazyDictionary);

		Assert.Equal(2, dictionary.Count);
	}
}
