// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using TypeCache.Adapters;
using TypeCache.Collections;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Adapters;

public class DictionaryAdapterTests
{
	private readonly Dictionary<int, string> _Items = new()
	{
		{ 1, "aaa" },
		{ 2, "bbb" },
		{ 3, "ccc" },
		{ 4, "ddd" },
		{ 5, "eee" },
		{ 6, "fff" },
	};

	[Fact]
	public void CountTest()
	{
		var adapter = new DictionaryAdapter(this._Items);

		Assert.Equal(this._Items.Count, adapter.Count);
	}

	[Fact]
	public void IsReadOnlyTest()
	{
		var adapter = new DictionaryAdapter(this._Items);

		Assert.Equal(false, adapter.IsReadOnly);
	}

	[Fact]
	public void KeysTest()
	{
		var adapter = new DictionaryAdapter(this._Items);

		Assert.Equivalent(adapter.Keys, new[] { 1, 2, 3, 4, 5, 6 });
	}

	[Fact]
	public void ValuesTest()
	{
		var adapter = new DictionaryAdapter(this._Items);

		Assert.Equivalent(adapter.Values, new[] { "aaa", "bbb", "ccc", "ddd", "eee", "fff" });
	}

	[Fact]
	public void DefaultIndexerTest()
	{
		var adapter = new DictionaryAdapter(this._Items);

		adapter[3] = "999999";
		Assert.Equal("999999", adapter[3]);
	}

	[Fact]
	public void AddTest()
	{
		var adapter = new DictionaryAdapter(this._Items);
		adapter.Add(7, "ggg");

		Assert.Equal(this._Items.Count, adapter.Count);
		Assert.Throws<NotImplementedException>(() => adapter.Add(KeyValuePair.Create<object, object>(8, "hhh")));
	}

	[Fact]
	public void ClearTest()
	{
		var adapter = new DictionaryAdapter(this._Items);
		adapter.Clear();

		Assert.Empty(this._Items);
	}

	[Fact]
	public void ContainsTest()
	{
		var adapter = new DictionaryAdapter(this._Items);

		Assert.Throws<NotImplementedException>(() => adapter.Contains(KeyValuePair.Create<object, object>(3, "ccc")));
	}

	[Fact]
	public void ContainsKeyTest()
	{
		var adapter = new DictionaryAdapter(this._Items);

		Assert.True(adapter.ContainsKey(4));
	}

	[Fact]
	public void CopyToTest()
	{
		var adapter = new DictionaryAdapter(this._Items);

		Assert.Throws<NotImplementedException>(() => adapter.CopyTo(Array<KeyValuePair<object, object>>.Empty, 2));
	}

	[Fact]
	public void GetEnumeratorTest()
	{
		var adapter = new DictionaryAdapter(this._Items);

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
		var adapter = new DictionaryAdapter(this._Items);
		adapter.Remove(5);

		Assert.Equal(this._Items.Count, adapter.Count);
	}
}
