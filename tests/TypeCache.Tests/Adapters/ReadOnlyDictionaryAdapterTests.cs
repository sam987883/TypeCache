// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using TypeCache.Adapters;
using TypeCache.Collections;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Adapters;

public class ReadOnlyDictionaryAdapterTests
{
	private readonly IReadOnlyDictionary<int, string> _Items = new Dictionary<int, string>()
	{
		{ 1, "aaa" },
		{ 2, "bbb" },
		{ 3, "ccc" },
		{ 4, "ddd" },
		{ 5, "eee" },
		{ 6, "fff" },
	}.AsReadOnly();

	[Fact]
	public void CountTest()
	{
		var adapter = new ReadOnlyDictionaryAdapter(this._Items);

		Assert.Equal(this._Items.Count, adapter.Count);
	}

	[Fact]
	public void KeysTest()
	{
		var adapter = new ReadOnlyDictionaryAdapter(this._Items);

		Assert.Equivalent(adapter.Keys, new[] { 1, 2, 3, 4, 5, 6 });
	}

	[Fact]
	public void ValuesTest()
	{
		var adapter = new ReadOnlyDictionaryAdapter(this._Items);

		Assert.Equivalent(adapter.Values, new[] { "aaa", "bbb", "ccc", "ddd", "eee", "fff" });
	}

	[Fact]
	public void DefaultIndexerTest()
	{
		var adapter = new ReadOnlyDictionaryAdapter(this._Items);

		Assert.Equal("ccc", adapter[3]);
	}

	[Fact]
	public void ContainsKeyTest()
	{
		var adapter = new ReadOnlyDictionaryAdapter(this._Items);

		Assert.True(adapter.ContainsKey(4));
	}

	[Fact]
	public void GetEnumeratorTest()
	{
		var adapter = new ReadOnlyDictionaryAdapter(this._Items);

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
