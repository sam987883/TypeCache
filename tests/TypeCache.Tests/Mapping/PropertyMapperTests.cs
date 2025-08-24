// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using TypeCache.Attributes;
using TypeCache.Extensions;
using TypeCache.Mapping;
using Xunit;

namespace TypeCache.Tests.Mapping;

public class PropertyMapperTests
{
	[Fact]
	public void MapDictionaryToModelProperties()
	{
		var dictionary = new Dictionary<string, object>(StringComparer.Ordinal)
		{
			{ nameof(TestModel1.TestProperty1), -1 },
			{ nameof(TestModel1.TestProperty2), ' ' },
			{ nameof(TestModel1.TestProperty3), null },
			{ nameof(TestModel1.TestProperty4), 101 },
			{ nameof(TestModel1.TestProperty5), 'X' },
			{ nameof(TestModel1.TestProperty6), "ABCdef" },
		};

		var testModel2 = new TestModel2();
		dictionary.MapProperties(testModel2);

		Assert.Equal(-1, testModel2.TestProperty1);
		Assert.Equal(' ', testModel2.TestProperty2);
		Assert.Equal(null, testModel2.TestProperty3);
		Assert.Equal(101, testModel2.TestProperty4);
		Assert.Equal('X', testModel2.TestProperty5);
		Assert.Equal("ABCdef", testModel2.TestProperty6);
	}

	[Fact]
	public void MapModelToModelProperties()
	{
		var mapper = new PropertyMapper();
		var testModel1 = new TestModel1
		{
			TestProperty1 = 1,
			TestProperty2 = '2',
			TestProperty3 = "333",
			TestProperty4 = 4,
			TestProperty5 = '5',
			TestProperty6 = "666666"
		};

		var testModel2 = new TestModel2();
		mapper.Map(testModel1, testModel2);

		Assert.Equal(1, testModel2.TestProperty4);
		Assert.Equal('2', testModel2.TestProperty2);
		Assert.Equal("333", testModel2.TestProperty3);
		Assert.Equal(4, testModel2.TestProperty1);
		Assert.Equal('5', testModel2.TestProperty5);
		Assert.Equal("666666", testModel2.TestProperty6);
	}

	private class TestModel1
	{
		[Map<TestModel2>(nameof(TestModel2.TestProperty4))]
		public int TestProperty1 { get; set; }
		public char TestProperty2 { get; set; }
		public string TestProperty3 { get; set; }
		[Map<TestModel2>(nameof(TestModel2.TestProperty1))]
		public int TestProperty4 { get; set; }
		public char TestProperty5 { get; set; }
		public string TestProperty6 { get; set; }
	}

	private class TestModel2
	{
		public int TestProperty1 { get; set; }
		public char TestProperty2 { get; set; }
		public string TestProperty3 { get; set; }
		public int TestProperty4 { get; set; }
		public char TestProperty5 { get; set; }
		public string TestProperty6 { get; set; }
	}
}
