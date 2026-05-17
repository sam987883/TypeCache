// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;
using Xunit;

namespace TypeCache.Tests.Collections;

public class LazyDictionaryTests
{
	[Fact]
	public void Create()
	{
		var dictionary = LazyDictionary.Create<string, int>(key => key.Length);

		Assert.NotNull(dictionary);
		Assert.Equal(0, dictionary.Count);
	}

	[Fact]
	public void Create_WithCapacity()
	{
		var dictionary = LazyDictionary.Create<string, int>(key => key.Length, capacity: 10);

		Assert.NotNull(dictionary);
		Assert.Equal(0, dictionary.Count);
	}

	[Fact]
	public void Create_WithComparer()
	{
		var dictionary = LazyDictionary.Create<string, int>(key => key.Length, comparer: StringComparer.OrdinalIgnoreCase);

		Assert.NotNull(dictionary);
		Assert.Equal(0, dictionary.Count);
	}

	[Fact]
	public void Indexer_CreateValue()
	{
		var dictionary = LazyDictionary.Create<string, int>(key => key.Length);

		var length = dictionary["hello"];
		Assert.Equal(5, length);
	}

	[Fact]
	public void Indexer_CachesValue()
	{
		var callCount = 0;
		var dictionary = LazyDictionary.Create<string, int>(key =>
		{
			callCount++;
			return key.Length;
		});

		var length1 = dictionary["hello"];
		var length2 = dictionary["hello"];

		Assert.Equal(1, callCount);
		Assert.Equal(5, length1);
		Assert.Equal(5, length2);
	}

	[Fact]
	public void ContainsKey_ExistingKey()
	{
		var dictionary = LazyDictionary.Create<string, int>(key => key.Length);
		_ = dictionary["hello"];

		Assert.True(dictionary.ContainsKey("hello"));
	}

	[Fact]
	public void ContainsKey_NonExistingKey()
	{
		var dictionary = LazyDictionary.Create<string, int>(key => key.Length);

		Assert.False(dictionary.ContainsKey("hello"));
	}

	[Fact]
	public void Keys()
	{
		var dictionary = LazyDictionary.Create<string, int>(key => key.Length);
		_ = dictionary["hello"];
		_ = dictionary["world"];

		Assert.Equal(2, dictionary.Count);
		Assert.Contains("hello", dictionary.Keys);
		Assert.Contains("world", dictionary.Keys);
	}

	[Fact]
	public void Values()
	{
		var dictionary = LazyDictionary.Create<string, int>(key => key.Length);
		_ = dictionary["hello"];
		_ = dictionary["world"];

		Assert.Equal(2, dictionary.Count);
		Assert.Contains(5, dictionary.Values);
	}

	[Fact]
	public void Count()
	{
		var dictionary = LazyDictionary.Create<string, int>(key => key.Length);

		Assert.Equal(0, dictionary.Count);

		_ = dictionary["hello"];
		Assert.Equal(1, dictionary.Count);

		_ = dictionary["world"];
		Assert.Equal(2, dictionary.Count);
	}

	[Fact]
	public void GetEnumerator()
	{
		var dictionary = LazyDictionary.Create<string, int>(key => key.Length);
		_ = dictionary["hello"];
		_ = dictionary["world"];

		Assert.Equal(2, dictionary.Count);
	}

	[Fact]
	public void CreateThreadSafe()
	{
		var dictionary = LazyDictionary.CreateThreadSafe<string, int>(key => key.Length);

		Assert.NotNull(dictionary);
		Assert.Equal(0, dictionary.Count);
	}

	[Fact]
	public void TryGetValue()
	{
		var dictionary = LazyDictionary.Create<string, int>(key => key.Length);

		var success = (dictionary as IReadOnlyDictionary<string, int>).TryGetValue("hello", out var length);

		Assert.True(success);
		Assert.Equal(5, length);
	}
}
