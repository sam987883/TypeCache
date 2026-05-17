// Copyright (c) 2021 Samuel Abraham

using TypeCache.Reflection;
using Xunit;

namespace TypeCache.Tests.Reflection;

public class TypeStoreTests
{
	[Fact]
	public void Attributes_Contains_IntAttributes()
	{
		var attributes = TypeStore.Attributes[typeof(int).TypeHandle];

		Assert.NotNull(attributes);
	}

	[Fact]
	public void ClrTypes_ContainsStringType()
	{
		var clrType = TypeStore.ClrTypes[typeof(string).TypeHandle];

		Assert.Equal(ClrType.Class, clrType);
	}

	[Fact]
	public void CodeNames_ContainsIntName()
	{
		var codeName = TypeStore.CodeNames[typeof(int).TypeHandle];

		Assert.NotEmpty(codeName);
	}

	[Fact]
	public void CollectionTypes_ContainsListType()
	{
		var collectionType = TypeStore.CollectionTypes[typeof(List<int>).TypeHandle];

		Assert.NotNull(collectionType);
	}

	[Fact]
	public void ObjectTypes_ContainsListObjectType()
	{
		var objectType = TypeStore.ObjectTypes[typeof(List<int>).TypeHandle];

		Assert.Equal(ObjectType.Enumerable, objectType);
	}

	[Fact]
	public void ScalarTypes_ContainsIntScalarType()
	{
		var scalarType = TypeStore.ScalarTypes[typeof(int).TypeHandle];

		Assert.NotNull(scalarType);
	}

	[Fact]
	public void Constructors_ContainsListConstructors()
	{
		var constructors = TypeStore.Constructors[typeof(List<int>).TypeHandle];

		Assert.NotNull(constructors);
	}

	[Fact]
	public void Properties_ContainsStringProperties()
	{
		var properties = TypeStore.Properties[typeof(string).TypeHandle];

		Assert.NotNull(properties);
	}

	[Fact]
	public void Fields_ContainsClassFields()
	{
		var fields = TypeStore.Fields[typeof(string).TypeHandle];

		Assert.NotNull(fields);
	}

	[Fact]
	public void GENERIC_TICKMARK_Constant()
	{
		Assert.Equal('`', TypeStore.GENERIC_TICKMARK);
	}
}
