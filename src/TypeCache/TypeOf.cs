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
		private static readonly TypeMember TypeMember = typeof(T).GetTypeMember();

		public static IImmutableList<Attribute> Attributes => TypeMember.Attributes;

		public static RuntimeTypeHandle BaseTypeHandle => TypeMember.BaseTypeHandle;

		public static TypeMember BaseType { get; } = TypeMember.BaseTypeHandle.GetTypeMember();

		public static TypeMember? EnclosedType { get; } = TypeMember.EnclosedTypeHandle?.GetTypeMember();

		public static IImmutableList<TypeMember> GenericTypes { get; } = TypeMember.GenericTypeHandles.To(handle => handle.GetTypeMember()).ToImmutableArray();

		public static RuntimeTypeHandle Handle => TypeMember.Handle;

		public static IImmutableList<TypeMember> InterfaceTypes { get; } = TypeMember.InterfaceTypeHandles.To(handle => handle.GetTypeMember()).ToImmutableArray();

		public static bool Is<V>() => TypeMember.Handle.Is<V>();

		public static bool Is(Type type) => TypeMember.Handle.Is(type);

		public static bool IsEnumerable => TypeMember.IsEnumerable;

		public static bool IsInternal => TypeMember.IsInternal;

		public static bool IsPublic => TypeMember.IsPublic;

		public static Kind Kind => TypeMember.Kind;

		public static string Name => TypeMember.Name;

		public static SystemType SystemType => TypeMember.SystemType;

		public static IImmutableList<ConstructorMember> Constructors => TypeMember.Constructors;

		public static IImmutableDictionary<string, InstanceFieldMember> Fields => TypeMember.Fields;

		public static IImmutableList<IndexerMember> Indexers => TypeMember.Indexers;

		public static IImmutableDictionary<string, IImmutableList<InstanceMethodMember>> Methods => TypeMember.Methods;

		public static IImmutableDictionary<string, InstancePropertyMember> Properties => TypeMember.Properties;

		public static IImmutableDictionary<string, StaticFieldMember> StaticFields => TypeMember.StaticFields;

		public static IImmutableDictionary<string, IImmutableList<StaticMethodMember>> StaticMethods => TypeMember.StaticMethods;

		public static IImmutableDictionary<string, StaticPropertyMember> StaticProperties => TypeMember.StaticProperties;

		public static T Create(params object?[]? parameters)
			=> (T)TypeMember.Create(parameters);

		public static D? GetMethod<D>(string name)
			where D : Delegate
			=> TypeMember.GetMethod<D>(name);

		public static D? GetStaticMethod<D>(string name)
			where D : Delegate
			=> TypeMember.GetStaticMethod<D>(name);
	}
}
