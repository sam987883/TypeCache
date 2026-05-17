// Copyright (c) 2021 Samuel Abraham

using TypeCache.Reflection;
using Xunit;

namespace TypeCache.Tests.Reflection;

public class TypeTests
{
	[Fact]
	public void Type_Generic_Properties()
	{
		var assemblyName = Type<int>.AssemblyName;
		var clrType = Type<int>.ClrType;
		var name = Type<int>.Name;
		var ns = Type<int>.Namespace;

		Assert.NotEmpty(assemblyName);
		Assert.Equal(ClrType.Struct, clrType);
		Assert.Equal("Int32", name);
		Assert.Equal("System", ns);
	}

	[Fact]
	public void Type_String_Properties()
	{
		var assemblyName = Type<string>.AssemblyName;
		var clrType = Type<string>.ClrType;
		var name = Type<string>.Name;
		var ns = Type<string>.Namespace;
		var isPublic = Type<string>.IsPublic;

		Assert.NotEmpty(assemblyName);
		Assert.Equal(ClrType.Class, clrType);
		Assert.Equal("String", name);
		Assert.Equal("System", ns);
		Assert.True(isPublic);
	}

	[Fact]
	public void Type_Class_Properties()
	{
		var clrType = Type<List<int>>.ClrType;
		var name = Type<List<int>>.Name;
		var ns = Type<List<int>>.Namespace;

		Assert.Equal(ClrType.Class, clrType);
		Assert.Equal("List", name);
		Assert.Equal("System.Collections.Generic", ns);
	}

	[Fact]
	public void Type_Enum_Properties()
	{
		var clrType = Type<ClrType>.ClrType;
		var name = Type<ClrType>.Name;

		Assert.Equal(ClrType.Enum, clrType);
		Assert.Equal("ClrType", name);
	}

	[Fact]
	public void Type_CollectionType()
	{
		var collectionType = Type<List<int>>.CollectionType;

		Assert.NotNull(collectionType);
	}

	[Fact]
	public void Type_ObjectType()
	{
		var objectType = Type<List<int>>.ObjectType;

		Assert.NotNull(objectType);
		Assert.Equal(ObjectType.Enumerable, objectType);
	}

	[Fact]
	public void Type_Handle()
	{
		var handle1 = Type<int>.Handle;
		var handle2 = Type<int>.Handle;

		Assert.Equal(handle1, handle2);
	}

	[Fact]
	public void Type_GenericTypes()
	{
		var genericTypes = Type<List<int>>.GenericTypes;

		Assert.Single(genericTypes);
		Assert.Equal(typeof(int), genericTypes[0]);
	}

	[Fact]
	public void Type_IsGeneric()
	{
		Assert.False(Type<int>.IsGeneric);
		Assert.True(Type<List<int>>.IsGeneric);
	}

	[Fact]
	public void Type_IsPublic()
	{
		Assert.True(Type<int>.IsPublic);
		Assert.True(Type<string>.IsPublic);
	}

	[Fact]
	public void Type_Attributes()
	{
		var attributes = Type<List<int>>.Attributes;

		Assert.NotNull(attributes);
	}
}
