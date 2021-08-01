﻿// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Collections.Extensions
{
	public class IReadOnlyDictionaryExtensions
	{
		[Fact]
		public void Get()
		{
			IReadOnlyDictionary<int, string> intStringDictionary = new Dictionary<int, string>()
			{
				{ 1, "111" },
				{ 2, "222" },
				{ 3, "333" },
			};

			Assert.Equal("222", intStringDictionary.Get(2));
		}

		[Fact]
		public void GetValue()
		{
			IReadOnlyDictionary<string, int> stringIntDictionary = new Dictionary<string, int>()
			{
				{ "111", 1 },
				{ "222", 2 },
				{ "333", 3 },
			};

			Assert.Equal(2, stringIntDictionary.GetValue("222"));
		}

		[Fact]
		public void GetValues()
		{
			IReadOnlyDictionary<int, string> intStringDictionary = new Dictionary<int, string>()
			{
				{ 1, "111" },
				{ 2, "222" },
				{ 3, "333" },
			};

			Assert.Equal(new[] { "111", "222" }, intStringDictionary.GetValues(1, 2));
			Assert.Equal(new[] { "222", "111" }, intStringDictionary.GetValues((IEnumerable<int>)new[] { 2, 1 }));
		}
	}
}