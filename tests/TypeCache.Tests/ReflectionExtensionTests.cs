// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection.Extensions;
using Xunit;

namespace TypeCache.Tests
{
	public class ReflectionExtensionTests
	{
		[Fact]
		public void ObjectExtensionTests()
		{
			Assert.Equal(TypeOf<string>.Member, "AAA".GetTypeMember());
			Assert.Equal(TypeOf<string>.Member, typeof(string).GetTypeMember());
			Assert.Equal(TypeOf<string>.Member, typeof(string).GetMethods().First().GetTypeMember());

			var testModel = new TestModel
			{
				TestProperty1 = 1,
				TestProperty2 = '2',
				TestProperty3 = "333",
				TestProperty4 = 4,
				TestProperty5 = '5',
				TestProperty6 = "666666"
			};

			var testModelFields = new TestModel();
			testModelFields.MapFields(testModel);
			Assert.Equal(1, testModelFields.TestProperty1);
			Assert.Equal('2', testModelFields.TestProperty2);
			Assert.Equal("333", testModelFields.TestProperty3);

			testModelFields = new TestModel();
			testModelFields.ReadFields(new Dictionary<string, object>(StringComparer.Ordinal)
			{
				{ "_TestField1", 101 },
				{ "_TestField2", 'X' },
				{ "_TestField3", "ABCdef" },
			});
			Assert.Equal(101, testModelFields.TestProperty1);
			Assert.Equal('X', testModelFields.TestProperty2);
			Assert.Equal("ABCdef", testModelFields.TestProperty3);

			var testModelProperties = new TestModel();
			testModelProperties.MapProperties(testModel);
			Assert.Equal(1, testModelProperties.TestProperty1);
			Assert.Equal('2', testModelProperties.TestProperty2);
			Assert.Equal("333", testModelProperties.TestProperty3);
			Assert.Equal(4, testModelProperties.TestProperty4);
			Assert.Equal('5', testModelProperties.TestProperty5);
			Assert.Equal("666666", testModelProperties.TestProperty6);

			testModelFields = new TestModel();
			testModelFields.ReadProperties(new Dictionary<string, object>(StringComparer.Ordinal)
			{
				{ nameof(TestModel.TestProperty4), 101 },
				{ nameof(TestModel.TestProperty5), 'X' },
				{ nameof(TestModel.TestProperty6), "ABCdef" },
			});
			Assert.Equal(101, testModelFields.TestProperty4);
			Assert.Equal('X', testModelFields.TestProperty5);
			Assert.Equal("ABCdef", testModelFields.TestProperty6);
		}

		private class TestModel
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
}
