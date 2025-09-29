// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using TypeCache.Attributes;
using TypeCache.Extensions;
using TypeCache.Mapping;
using Xunit;

namespace TypeCache.Tests.Mapping;

public class FieldMapperTests
{
	[Fact]
	public void MapDictionaryToModelFields()
	{
		var dictionary = new Dictionary<string, object>(StringComparer.Ordinal)
		{
			{ nameof(TestModel1.TestField1), -1 },
			{ nameof(TestModel1.TestField2), ' ' },
			{ nameof(TestModel1.TestField3), null },
			{ nameof(TestModel1.TestField4), 101 },
			{ nameof(TestModel1.TestField5), 'X' },
			{ nameof(TestModel1.TestField6), "ABCdef" },
		};

		var testModel2 = new TestModel2();
		dictionary.MapFields(testModel2);

		Assert.Equal(-1, testModel2.TestField1);
		Assert.Equal(' ', testModel2.TestField2);
		Assert.Equal(null, testModel2.TestField3);
		Assert.Equal(101, testModel2.TestField4);
		Assert.Equal('X', testModel2.TestField5);
		Assert.Equal("ABCdef", testModel2.TestField6);
	}

	[Fact]
	public void MapModelToModelFields()
	{
		var mapper = new FieldMapper();
		var testModel1 = new TestModel1
		{
			TestField1 = 1,
			TestField2 = '2',
			TestField3 = "333",
			TestField4 = 4,
			TestField5 = '5',
			TestField6 = "666666"
		};

		var testModel2 = new TestModel2();
		mapper.Map(testModel1, testModel2);

		Assert.Equal(1, testModel2.TestField4);
		Assert.Equal('2', testModel2.TestField2);
		Assert.Equal("333", testModel2.TestField3);
		Assert.Equal(4, testModel2.TestField1);
		Assert.Equal('5', testModel2.TestField5);
		Assert.Equal("666666", testModel2.TestField6);
	}

	private class TestModel1
	{
		[MapAttribute<TestModel2>(nameof(TestModel2.TestField4))]
		public int TestField1 = default;
		public char TestField2 = default;
		public string TestField3 = default;
		[MapAttribute<TestModel2>(nameof(TestModel2.TestField1))]
		public int TestField4 = default;
		public char TestField5 = default;
		public string TestField6 = default;
	}

	private class TestModel2
	{
		public int TestField1 = default;
		public char TestField2 = default;
		public string TestField3 = default;
		public int TestField4 = default;
		public char TestField5 = default;
		public string TestField6 = default;
	}
}
