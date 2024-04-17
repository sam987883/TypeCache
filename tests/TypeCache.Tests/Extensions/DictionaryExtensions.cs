// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		Action<KeyValuePair<int, string>> actionPair = pair => pair.ToString();

		Assert.True(dictionary.If(1, action));
		Assert.False(dictionary.If(0, action));
		Assert.True(dictionary.If(2, actionPair));
		Assert.False(dictionary.If(-1, actionPair));
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
}
