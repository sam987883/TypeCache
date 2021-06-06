// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;

namespace TypeCache
{
	public static class TypeOf<T>
	{
		public static TypeMember Member { get; } = typeof(T).GetTypeMember();

		public static IImmutableList<Attribute> Attributes => Member.Attributes;

		public static RuntimeTypeHandle BaseTypeHandle => Member.BaseTypeHandle;

		public static TypeMember BaseType { get; } = Member.BaseTypeHandle.GetTypeMember();

		public static TypeMember? EnclosedType { get; } = Member.EnclosedTypeHandle?.GetTypeMember();

		public static IImmutableList<TypeMember> GenericTypes { get; } = Member.GenericTypeHandles.To(handle => handle.GetTypeMember()).ToImmutableArray();

		public static RuntimeTypeHandle Handle => Member.Handle;

		public static IImmutableList<TypeMember> InterfaceTypes { get; } = Member.InterfaceTypeHandles.To(handle => handle.GetTypeMember()).ToImmutableArray();

		public static bool Is<V>() => Member.Handle.Is<V>();

		public static bool Is(Type type) => Member.Handle.Is(type);

		public static bool IsEnumerable => Member.IsEnumerable;

		public static bool IsInternal => Member.IsInternal;

		public static bool IsPublic => Member.IsPublic;

		public static Kind Kind => Member.Kind;

		public static string Name => Member.Name;

		public static SystemType SystemType => Member.SystemType;

		public static IImmutableList<ConstructorMember> Constructors => Member.Constructors;

		public static IImmutableDictionary<string, InstanceFieldMember> Fields => Member.Fields;

		public static IImmutableList<IndexerMember> Indexers => Member.Indexers;

		public static IImmutableDictionary<string, IImmutableList<InstanceMethodMember>> Methods => Member.Methods;

		public static IImmutableDictionary<string, InstancePropertyMember> Properties => Member.Properties;

		public static IImmutableDictionary<string, StaticFieldMember> StaticFields => Member.StaticFields;

		public static IImmutableDictionary<string, IImmutableList<StaticMethodMember>> StaticMethods => Member.StaticMethods;

		public static IImmutableDictionary<string, StaticPropertyMember> StaticProperties => Member.StaticProperties;

		public static T Create(params object?[]? parameters)
			=> (T)Member.Create(parameters);

		public static D? GetMethod<D>(string name)
			where D : Delegate
			=> Member.GetMethod<D>(name);

		public static D? GetStaticMethod<D>(string name)
			where D : Delegate
			=> Member.GetStaticMethod<D>(name);
	}
}
