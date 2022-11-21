// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class DictionaryExtensions
{
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

		Assert.NotNull(intStringDictionary.ToReadOnly());
		Assert.NotNull(stringIntDictionary.ToReadOnly());
	}
}
