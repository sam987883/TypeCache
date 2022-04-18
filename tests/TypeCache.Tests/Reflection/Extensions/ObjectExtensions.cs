// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using TypeCache.Collections.Extensions;
using TypeCache.Mappers.Extensions;
using TypeCache.Reflection.Extensions;
using Xunit;

namespace TypeCache.Tests.Reflection.Extensions;

public class ObjectExtensions
{
	[Fact]
	public void GetTypeMember()
	{
		Assert.Equal(TypeOf<string>.Member, "AAA".GetTypeMember());
		Assert.Equal(TypeOf<string>.Member, typeof(string).GetTypeMember());
		Assert.Equal(TypeOf<string>.Member, typeof(string).GetMethods().First().GetTypeMember());
	}

	[Fact]
	public void MapFields()
	{
		var testModel1 = new TestModel1
		{
			TestProperty1 = 1,
			TestProperty2 = '2',
			TestProperty3 = "333",
		};

		var testModel2 = new TestModel2();
		var fields = testModel2.MapFields(testModel1).ToArray();
		Assert.Equal(1, testModel2.TestProperty1);
		Assert.Equal('2', testModel2.TestProperty2);
		Assert.Equal("333", testModel2.TestProperty3);
	}

	[Fact]
	public void MapProperties()
	{
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
		var properties = testModel2.MapProperties(testModel1).ToArray();
		Assert.Equal(1, testModel2.TestProperty1);
		Assert.Equal('2', testModel2.TestProperty2);
		Assert.Equal("333", testModel2.TestProperty3);
		Assert.Equal(4, testModel2.TestProperty4);
		Assert.Equal('5', testModel2.TestProperty5);
		Assert.Equal("666666", testModel2.TestProperty6);
	}

	[Fact]
	public void ReadFields()
	{
		var testModel1 = new TestModel1();
		IReadOnlyDictionary<string, object> dictionary = new Dictionary<string, object>(StringComparer.Ordinal)
		{
			{ "_TestField1", 101 },
			{ "_TestField2", 'X' },
			{ "_TestField3", "ABCdef" },
		};
		var fields = testModel1.MapFields(dictionary).ToArray();
		Assert.Equal(101, testModel1.TestProperty1);
		Assert.Equal('X', testModel1.TestProperty2);
		Assert.Equal("ABCdef", testModel1.TestProperty3);
	}

	[Fact]
	public void ReadProperties()
	{
		var testModel2 = new TestModel2();
		IReadOnlyDictionary<string, object> dictionary = new Dictionary<string, object>(StringComparer.Ordinal)
		{
			{ nameof(TestModel1.TestProperty4), 101 },
			{ nameof(TestModel1.TestProperty5), 'X' },
			{ nameof(TestModel1.TestProperty6), "ABCdef" },
		};
		var properties = testModel2.MapProperties(dictionary).ToArray();
		Assert.Equal(101, testModel2.TestProperty4);
		Assert.Equal('X', testModel2.TestProperty5);
		Assert.Equal("ABCdef", testModel2.TestProperty6);
	}

	private class TestModel1
	{
		private int _TestField1;
		private char _TestField2;
		private string _TestField3;

		public int TestProperty1 { get => this._TestField1; set => this._TestField1 = value; }
		public char TestProperty2 { get => this._TestField2; set => this._TestField2 = value; }
		public string TestProperty3 { get => this._TestField3; set => this._TestField3 = value; }
		public int TestProperty4 { get; set; }
		public char TestProperty5 { get; set; }
		public string TestProperty6 { get; set; }
	}

	private class TestModel2
	{
		private int _TestField1;
		private char _TestField2;
		private string _TestField3;

		public int TestProperty1 { get => this._TestField1; set => this._TestField1 = value; }
		public char TestProperty2 { get => this._TestField2; set => this._TestField2 = value; }
		public string TestProperty3 { get => this._TestField3; set => this._TestField3 = value; }
		public int TestProperty4 { get; set; }
		public char TestProperty5 { get; set; }
		public string TestProperty6 { get; set; }
	}
}
