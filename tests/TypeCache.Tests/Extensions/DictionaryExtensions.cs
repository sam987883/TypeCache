// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TypeCache.Collections;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class DictionaryExtensions
{
	[Fact]
	public void If()
	{
		var dictionary = new Dictionary<int, string>()
		{
			{ 1, "111" },
			{ 2, "222" },
			{ 3, "333" },
		};
		Action action = () => 0.ToString();
		Action<int, string> actionPair = (a, b) => (a, b).ToString();

		Assert.True(dictionary.If(1, action));
		Assert.False(dictionary.If(0, action));
		Assert.True(dictionary.If(2, actionPair));
		Assert.False(dictionary.If(-1, actionPair));
	}

	[Fact]
	public void Map()
	{
		var dictionary1 = new Dictionary<string, object>(StringComparer.Ordinal)
		{
			{ "Key1", -1 },
			{ "Key2", ' ' },
			{ "Key3", null },
			{ "Key4", 101 },
			{ "Key5", 'X' },
			{ "Key6", "ABCdef" },
		};

		var dictionary2 = new Dictionary<string, object>(StringComparer.Ordinal);
		dictionary1.Map(dictionary2);

		Assert.Equal(-1, dictionary2["Key1"]);
		Assert.Equal(' ', dictionary2["Key2"]);
		Assert.False(dictionary2.ContainsKey("Key3"));
		Assert.Equal(101, dictionary2["Key4"]);
		Assert.Equal('X', dictionary2["Key5"]);
		Assert.Equal("ABCdef", dictionary2["Key6"]);

		dictionary2.Clear();

		((IEnumerable<KeyValuePair<string, object>>)dictionary1).Map(dictionary2);

		Assert.Equal(-1, dictionary2["Key1"]);
		Assert.Equal(' ', dictionary2["Key2"]);
		Assert.False(dictionary2.ContainsKey("Key3"));
		Assert.Equal(101, dictionary2["Key4"]);
		Assert.Equal('X', dictionary2["Key5"]);
		Assert.Equal("ABCdef", dictionary2["Key6"]);
	}

	[Fact]
	public void MapFields()
	{
		var dictionary = new Dictionary<string, object>(StringComparer.Ordinal)
		{
			{ nameof(TestModelFields.TestField1), -1 },
			{ nameof(TestModelFields.TestField2), ' ' },
			{ nameof(TestModelFields.TestField3), null },
			{ nameof(TestModelFields.TestField4), 101 },
			{ nameof(TestModelFields.TestField5), 'X' },
			{ nameof(TestModelFields.TestField6), "ABCdef" },
		};

		var model = new TestModelFields();
		dictionary.MapFields(model);

		Assert.Equal(-1, model.TestField1);
		Assert.Equal(' ', model.TestField2);
		Assert.Null(model.TestField3);
		Assert.Equal(101, model.TestField4);
		Assert.Equal('X', model.TestField5);
		Assert.Equal("ABCdef", model.TestField6);

		model = new TestModelFields();
		((IEnumerable<KeyValuePair<string, object>>)dictionary).MapFields(model);

		Assert.Equal(-1, model.TestField1);
		Assert.Equal(' ', model.TestField2);
		Assert.Null(model.TestField3);
		Assert.Equal(101, model.TestField4);
		Assert.Equal('X', model.TestField5);
		Assert.Equal("ABCdef", model.TestField6);
	}

	[Fact]
	public void MapProperties()
	{
		var dictionary = new Dictionary<string, object>(StringComparer.Ordinal)
		{
			{ nameof(TestModelProperties.TestProperty1), -1 },
			{ nameof(TestModelProperties.TestProperty2), ' ' },
			{ nameof(TestModelProperties.TestProperty3), null },
			{ nameof(TestModelProperties.TestProperty4), 101 },
			{ nameof(TestModelProperties.TestProperty5), 'X' },
			{ nameof(TestModelProperties.TestProperty6), "ABCdef" },
		};

		var model = new TestModelProperties();
		dictionary.MapProperties(model);

		Assert.Equal(-1, model.TestProperty1);
		Assert.Equal(' ', model.TestProperty2);
		Assert.Null(model.TestProperty3);
		Assert.Equal(101, model.TestProperty4);
		Assert.Equal('X', model.TestProperty5);
		Assert.Equal("ABCdef", model.TestProperty6);

		model = new TestModelProperties();
		((IEnumerable<KeyValuePair<string, object>>)dictionary).MapProperties(model);

		Assert.Equal(-1, model.TestProperty1);
		Assert.Equal(' ', model.TestProperty2);
		Assert.Null(model.TestProperty3);
		Assert.Equal(101, model.TestProperty4);
		Assert.Equal('X', model.TestProperty5);
		Assert.Equal("ABCdef", model.TestProperty6);
	}

	[Fact]
	public void ToLazyReadOnly()
	{
		IDictionary<int, Lazy<string>> intStringDictionary = new Dictionary<int, Lazy<string>>()
		{
			{ 1, new Lazy<string>(() => "111") },
			{ 2, new Lazy<string>(() => "222") },
			{ 3, new Lazy<string>(() => "333") },
		};
		IDictionary<string, Lazy<int>> stringIntDictionary = new Dictionary<string, Lazy<int>>()
		{
			{ "111", new Lazy<int>(() => 1) },
			{ "222", new Lazy<int>(() => 2) },
			{ "333", new Lazy<int>(() => 3) },
		};

		var intReadOnlyDictionary = intStringDictionary.ToReadOnly().ToLazyReadOnly();
		Assert.NotNull(intReadOnlyDictionary);
		Assert.IsType<LazyReadOnlyDictionary<int, string>>(intReadOnlyDictionary);

		var stringReadOnlyDictionary = stringIntDictionary.ToReadOnly().ToLazyReadOnly();
		Assert.NotNull(stringReadOnlyDictionary);
		Assert.IsType<LazyReadOnlyDictionary<string, int>>(stringReadOnlyDictionary);
	}

	[Fact]
	public void ToReadOnly()
	{
		IDictionary<int, string> intStringDictionary = new Dictionary<int, string>()
		{
			{ 1, "111" },
			{ 2, "222" },
			{ 3, "333" },
		};
		IDictionary<string, int> stringIntDictionary = new Dictionary<string, int>()
		{
			{ "111", 1 },
			{ "222", 2 },
			{ "333", 3 },
		};

		var intReadOnlyDictionary = intStringDictionary.ToReadOnly();
		Assert.NotNull(intReadOnlyDictionary);
		Assert.IsType<ReadOnlyDictionary<int, string>>(intReadOnlyDictionary);

		var stringReadOnlyDictionary = stringIntDictionary.ToReadOnly();
		Assert.NotNull(stringReadOnlyDictionary);
		Assert.IsType<ReadOnlyDictionary<string, int>>(stringReadOnlyDictionary);
	}

	private class TestModelFields
	{
		public int TestField1 = default;
		public char TestField2 = default;
		public string TestField3 = default;
		public int TestField4 = default;
		public char TestField5 = default;
		public string TestField6 = default;
	}

	private class TestModelProperties
	{
		public int TestProperty1 { get; set; }
		public char TestProperty2 { get; set; }
		public string TestProperty3 { get; set; }
		public int TestProperty4 { get; set; }
		public char TestProperty5 { get; set; }
		public string TestProperty6 { get; set; }
	}
}
