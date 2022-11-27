// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TypeCache.Data;
using TypeCache.Extensions;
using TypeCache.Reflection;
using Xunit;

namespace TypeCache.Tests;

public class Tester1
{
	public A GenericMethod<A, B, C>(B param1, C param2)
	{
		Console.WriteLine($"{param1?.ToString() ?? "null"} and {param2?.ToString() ?? "null"}");
		return default(A);
	}
}

public class TypeOfTests
{
	[Fact]
	public void InvokeGenericMethod()
	{
		var tester = new Tester1();

		var method = TypeOf<Tester1>.Methods.Where(_ => _.Name.Is(nameof(Tester1.GenericMethod))).FirstOrDefault();
		Assert.NotEqual(default, method);

		var value = method.InvokeGeneric(new[] { typeof(int), typeof(string), typeof(DateTime) }, tester, "param1", DateTime.Now);
		Assert.Equal(0, value);

		// Test pulling it out of cache
		method = TypeOf<Tester1>.Methods.Where(_ => _.Name.Is(nameof(Tester1.GenericMethod))).FirstOrDefault();
		Assert.NotEqual(default, method);

		value = method.InvokeGeneric(new[] { typeof(int), typeof(string), typeof(DateTime) }, tester, "param1", DateTime.Now);
		Assert.Equal(0, value);
	}

	[Fact]
	public void TypeOfClass()
	{
		var member = TypeOf<object>.Member;
		var type = typeof(object);

		Assert.Equal(1, member.Attributes.Count);
		Assert.Null(member.BaseType);
		Assert.Equal(1, member.Constructors.Count);
		Assert.Null(member.ElementType);
		Assert.Empty(member.Events);
		Assert.Empty(member.Fields);
		Assert.Empty(member.GenericTypes);
		Assert.Equal(type.TypeHandle, member.TypeHandle);
		Assert.Empty(member.InterfaceTypes);
		Assert.False(member.Internal);
		Assert.Equal(Kind.Class, member.Kind);
		Assert.Equal(8, member.Methods.Count);
		Assert.Equal(type.Name, member.Name);
		Assert.Empty(member.Properties);
		Assert.True(member.Public);
		Assert.Equal(type.IsByRef || type.IsByRefLike, member.Ref);
		Assert.Equal(SystemType.Object, member.SystemType);

		member = TypeOf<string>.Member;
		type = typeof(string);

		Assert.Equal(6, member.Attributes.Count);
		Assert.Equal(typeof(object), member.BaseType);
		Assert.Equal(3, member.Constructors.Count);
		Assert.Null(member.ElementType);
		Assert.Empty(member.Events);
		Assert.Equal(3, member.Fields.Count);
		Assert.Empty(member.GenericTypes);
		Assert.Equal(type.TypeHandle, member.TypeHandle);
		Assert.Equal(7, member.InterfaceTypes.Count);
		Assert.False(member.Internal);
		Assert.Equal(Kind.Class, member.Kind);
		Assert.Equal(200, member.Methods.Count);
		Assert.Equal(type.Name, member.Name);
		Assert.Equal(2, member.Properties.Count);
		Assert.True(member.Public);
		Assert.Equal(type.IsByRef || type.IsByRefLike, member.Ref);
		Assert.Equal(SystemType.String, member.SystemType);

		member = TypeOf<SqlCommand>.Member;
		type = typeof(SqlCommand);

		Assert.Equal(2, member.Attributes.Count);
		Assert.Equal(typeof(object), member.BaseType);
		Assert.Equal(1, member.Constructors.Count);
		Assert.Null(member.ElementType);
		Assert.Empty(member.Events);
		Assert.Equal(10, member.Fields.Count);
		Assert.Empty(member.GenericTypes);
		Assert.Equal(type.TypeHandle, member.TypeHandle);
		Assert.Equal(0, member.InterfaceTypes.Count);
		Assert.False(member.Internal);
		Assert.Equal(Kind.Class, member.Kind);
		Assert.Equal(9, member.Methods.Count);
		Assert.Equal(type.Name, member.Name);
		Assert.Equal(10, member.Properties.Count);
		Assert.True(member.Public);
		Assert.Equal(type.IsByRef || type.IsByRefLike, member.Ref);
		Assert.Equal(SystemType.None, member.SystemType);
	}

	[Fact]
	public void TypeOfCollection()
	{
		var member = TypeOf<ImmutableArray<string>>.Member;
		var type = typeof(ImmutableArray<string>);

		Assert.Equal(6, member.Attributes.Count);
		Assert.Equal(typeof(ValueType), member.BaseType);
		Assert.Equal(1, member.Constructors.Count);
		Assert.Null(member.ElementType);
		Assert.Empty(member.Events);
		Assert.Equal(2, member.Fields.Count);
		Assert.Contains(TypeOf<string>.Member, member.GenericTypes);
		Assert.Equal(type.TypeHandle, member.TypeHandle);
		Assert.Equal(13, member.InterfaceTypes.Count);
		Assert.False(member.Internal);
		Assert.True(member.SystemType.IsCollection());
		Assert.True(member.SystemType.IsImmutable());
		Assert.Equal(93, member.Methods.Count);
		Assert.Contains(member.Name, type.Name);
		Assert.Equal(18, member.Properties.Count);
		Assert.True(member.Public);
		Assert.Equal(type.IsByRef || type.IsByRefLike, member.Ref);
		Assert.Equal(SystemType.ImmutableArray, member.SystemType);

		member = TypeOf<IList<ulong>>.Member;
		type = typeof(IList<ulong>);

		Assert.Equal(2, member.Attributes.Count);
		Assert.Null(member.BaseType);
		Assert.Empty(member.Constructors);
		Assert.Null(member.ElementType);
		Assert.Empty(member.Events);
		Assert.Empty(member.Fields);
		Assert.Contains(TypeOf<ulong>.Member, member.GenericTypes);
		Assert.Equal(type.TypeHandle, member.TypeHandle);
		Assert.Equal(3, member.InterfaceTypes.Count);
		Assert.False(member.Internal);
		Assert.False(member.SystemType.IsCollection());
		Assert.False(member.SystemType.IsImmutable());
		Assert.Equal(3, member.Methods.Count);
		Assert.Contains(member.Name, type.Name);
		Assert.Equal(ObjectType.List, member.ObjectType);
		Assert.Equal(1, member.Properties.Count);
		Assert.True(member.Public);
		Assert.Equal(type.IsByRef || type.IsByRefLike, member.Ref);
	}

	[Fact]
	public void TypeOfDelegate()
	{
		var member = TypeOf<Predicate<string>>.Member;
		var type = typeof(Predicate<string>);

		Assert.Empty(member.Attributes);
		Assert.Equal(typeof(MulticastDelegate), member.BaseType);
		Assert.Equal(1, member.Constructors.Count);
		Assert.Null(member.ElementType);
		Assert.Empty(member.Events);
		Assert.Empty(member.Fields);
		Assert.Contains(TypeOf<string>.Member, member.GenericTypes);
		Assert.Equal(type.TypeHandle, member.TypeHandle);
		Assert.Equal(2, member.InterfaceTypes.Count);
		Assert.False(member.Internal);
		Assert.Equal(Kind.Class, member.Kind);
		Assert.Equal(48, member.Methods.Count);
		Assert.Contains(member.Name, type.Name);
		Assert.Equal(ObjectType.Delegate, member.ObjectType);
		Assert.Equal(2, member.Properties.Count);
		Assert.True(member.Public);
		Assert.Equal(type.IsByRef || type.IsByRefLike, member.Ref);
		Assert.Equal(SystemType.None, member.SystemType);
	}

	[Fact]
	public void TypeOfEnum()
	{
		var member = TypeOf<SystemType>.Member;
		var type = typeof(SystemType);

		Assert.Equal(0, member.Attributes.Count);
		Assert.Equal(typeof(Enum), member.BaseType);
		Assert.Empty(member.Constructors);
		Assert.Null(member.ElementType);
		Assert.Equal(0, member.Events.Count);
		Assert.Equal(1, member.Fields.Count);
		Assert.Empty(member.GenericTypes);
		Assert.Equal(type.TypeHandle, member.TypeHandle);
		Assert.Equal(3, member.InterfaceTypes.Count);
		Assert.False(member.Internal);
		Assert.Equal(Kind.Struct, member.Kind);
		Assert.Equal(60, member.Methods.Count);
		Assert.Equal(type.Name, member.Name);
		Assert.Equal(ObjectType.Enum, member.ObjectType);
		Assert.Empty(member.Properties);
		Assert.True(member.Public);
		Assert.Equal(type.IsByRef || type.IsByRefLike, member.Ref);
		Assert.Equal(SystemType.Int32, member.SystemType);
	}

	[Fact]
	public void TypeOfInterface()
	{
		var member = TypeOf<IAccessor<IDataSource>>.Member;
		var type = typeof(IAccessor<IDataSource>);

		Assert.Equal(2, member.Attributes.Count);
		Assert.Null(member.BaseType);
		Assert.Empty(member.Constructors);
		Assert.Null(member.ElementType);
		Assert.Empty(member.Events);
		Assert.Empty(member.Fields);
		Assert.Equal(1, member.GenericTypes.Count);
		Assert.Equal(type.TypeHandle, member.TypeHandle);
		Assert.Empty(member.InterfaceTypes);
		Assert.False(member.Internal);
		Assert.Equal(Kind.Interface, member.Kind);
		Assert.Equal(1, member.Methods.Count);
		Assert.True(type.Name.StartsWith(member.Name, StringComparison.Ordinal));
		Assert.Equal(1, member.Properties.Count);
		Assert.True(member.Public);
		Assert.Equal(type.IsByRef || type.IsByRefLike, member.Ref);
		Assert.Equal(SystemType.None, member.SystemType);
	}

	[Fact]
	public void TypeOfPointer()
	{
		var type = typeof(void*);
		var member = type.GetTypeMember();

		Assert.Empty(member.Attributes);
		Assert.Null(member.BaseType);
		Assert.Empty(member.Constructors);
		Assert.Equal(typeof(void).GetTypeMember(), member.ElementType);
		Assert.Empty(member.Events);
		Assert.Empty(member.Fields);
		Assert.Empty(member.GenericTypes);
		Assert.Equal(type.TypeHandle, member.TypeHandle);
		Assert.Empty(member.InterfaceTypes);
		Assert.False(member.Internal);
		Assert.Equal(Kind.Pointer, member.Kind);
		Assert.Empty(member.Methods);
		Assert.Equal(type.Name, member.Name);
		Assert.Empty(member.Properties);
		Assert.True(member.Public);
		Assert.Equal(type.IsByRef || type.IsByRefLike, member.Ref);
		Assert.Equal(SystemType.None, member.SystemType);

		type = typeof(int*);
		member = type.GetTypeMember();

		Assert.Empty(member.Attributes);
		Assert.Null(member.BaseType);
		Assert.Empty(member.Constructors);
		Assert.Equal(typeof(int).GetTypeMember(), member.ElementType);
		Assert.Empty(member.Events);
		Assert.Empty(member.Fields);
		Assert.Empty(member.GenericTypes);
		Assert.Equal(type.TypeHandle, member.TypeHandle);
		Assert.Empty(member.InterfaceTypes);
		Assert.False(member.Internal);
		Assert.Equal(Kind.Pointer, member.Kind);
		Assert.Empty(member.Methods);
		Assert.Equal(type.Name, member.Name);
		Assert.Empty(member.Properties);
		Assert.True(member.Public);
		Assert.Equal(type.IsByRef || type.IsByRefLike, member.Ref);
		Assert.Equal(SystemType.None, member.SystemType);

		type = typeof(bool*);
		member = type.GetTypeMember();

		Assert.Empty(member.Attributes);
		Assert.Null(member.BaseType);
		Assert.Empty(member.Constructors);
		Assert.Equal(typeof(bool).GetTypeMember(), member.ElementType);
		Assert.Empty(member.Events);
		Assert.Empty(member.Fields);
		Assert.Empty(member.GenericTypes);
		Assert.Equal(type.TypeHandle, member.TypeHandle);
		Assert.Empty(member.InterfaceTypes);
		Assert.False(member.Internal);
		Assert.Equal(Kind.Pointer, member.Kind);
		Assert.Empty(member.Methods);
		Assert.Equal(type.Name, member.Name);
		Assert.Empty(member.Properties);
		Assert.True(member.Public);
		Assert.Equal(type.IsByRef || type.IsByRefLike, member.Ref);
		Assert.Equal(SystemType.None, member.SystemType);
	}

	[Fact]
	public void TypeOfStruct()
	{
		var member = TypeOf<int>.Member;
		var type = typeof(int);

		Assert.Equal(type.GetCustomAttributes(true).Length, member.Attributes.Count);
		Assert.Equal(typeof(ValueType), member.BaseType);
		Assert.Empty(member.Constructors);
		Assert.Null(member.ElementType);
		Assert.Empty(member.Events);
		Assert.Equal(1, member.Fields.Count);
		Assert.Empty(member.GenericTypes);
		Assert.Equal(type.TypeHandle, member.TypeHandle);
		Assert.Equal(29, member.InterfaceTypes.Count);
		Assert.False(member.Internal);
		Assert.Equal(Kind.Struct, member.Kind);
		Assert.Equal(104, member.Methods.Count);
		Assert.Equal(type.Name, member.Name);
		Assert.True(member.Public);
		Assert.Equal(type.IsByRef || type.IsByRefLike, member.Ref);
		Assert.Equal(SystemType.Int32, member.SystemType);

		member = TypeOf<bool>.Member;
		type = typeof(bool);

		Assert.Equal(3, member.Attributes.Count);
		Assert.Equal(typeof(ValueType), member.BaseType);
		Assert.Empty(member.Constructors);
		Assert.Null(member.ElementType);
		Assert.Empty(member.Events);
		Assert.Equal(3, member.Fields.Count);
		Assert.Empty(member.GenericTypes);
		Assert.Equal(type.TypeHandle, member.TypeHandle);
		Assert.Equal(4, member.InterfaceTypes.Count);
		Assert.False(member.Internal);
		Assert.Equal(Kind.Struct, member.Kind);
		Assert.Equal(29, member.Methods.Count);
		Assert.Equal(type.Name, member.Name);
		Assert.Empty(member.Properties);
		Assert.True(member.Public);
		Assert.Equal(type.IsByRef || type.IsByRefLike, member.Ref);
		Assert.Equal(SystemType.Boolean, member.SystemType);

		member = TypeOf<int?>.Member;
		type = typeof(int?);

		Assert.Equal(3, member.Attributes.Count);
		Assert.Equal(typeof(ValueType), member.BaseType);
		Assert.Equal(1, member.Constructors.Count);
		Assert.Null(member.ElementType);
		Assert.Empty(member.Events);
		Assert.Equal(2, member.Fields.Count);
		Assert.Contains(TypeOf<int>.Member, member.GenericTypes);
		Assert.Equal(type.TypeHandle, member.TypeHandle);
		Assert.Empty(member.InterfaceTypes);
		Assert.False(member.Internal);
		Assert.Equal(Kind.Struct, member.Kind);
		Assert.Equal(10, member.Methods.Count);
		Assert.Contains(member.Name, type.Name);
		Assert.Equal(2, member.Properties.Count);
		Assert.True(member.Public);
		Assert.Equal(type.IsByRef || type.IsByRefLike, member.Ref);
		Assert.Equal(SystemType.Nullable, member.SystemType);

		member = TypeOf<ColumnSchema>.Member;
		type = typeof(ColumnSchema);

		Assert.Equal(2, member.Attributes.Count);
		Assert.Equal(typeof(object), member.BaseType);
		Assert.Equal(2, member.Constructors.Count);
		Assert.Null(member.ElementType);
		Assert.Empty(member.Events);
		Assert.Equal(6, member.Fields.Count);
		Assert.Empty(member.GenericTypes);
		Assert.Equal(type.TypeHandle, member.TypeHandle);
		Assert.Equal(1, member.InterfaceTypes.Count);
		Assert.False(member.Internal);
		Assert.Equal(Kind.Class, member.Kind);
		Assert.Equal(11, member.Methods.Count);
		Assert.Equal(type.Name, member.Name);
		Assert.Equal(7, member.Properties.Count);
		Assert.True(member.Public);
		Assert.Equal(type.IsByRef || type.IsByRefLike, member.Ref);
		Assert.Equal(SystemType.None, member.SystemType);
	}
}
