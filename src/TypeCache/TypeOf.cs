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
		private static readonly TypeMember Type = MemberCache.Types[typeof(T).TypeHandle];

		public static IImmutableList<Attribute> Attributes => Type.Attributes;

		public static RuntimeTypeHandle? BaseTypeHandle => Type.BaseTypeHandle;

		public static TypeMember? BaseType => Type.BaseTypeHandle.HasValue ? MemberCache.Types[Type.BaseTypeHandle.Value] : null;

		public static IImmutableList<RuntimeTypeHandle> GenericTypeHandles => Type.GenericTypeHandles;

		public static TypeMember[] GenericTypes => Type.GenericTypeHandles.To(handle => MemberCache.Types[handle]).ToArray();

		public static RuntimeTypeHandle Handle => Type.Handle;

		public static IImmutableList<RuntimeTypeHandle> InterfaceTypeHandles => Type.InterfaceTypeHandles;

		public static TypeMember[] InterfaceTypes => Type.InterfaceTypeHandles.To(handle => MemberCache.Types[handle]).ToArray();

		public static bool IsEnumerable => Type.IsEnumerable;

		public static bool IsInternal => Type.IsInternal;

		public static bool IsPublic => Type.IsPublic;

		public static Kind Kind => Type.Kind;

		public static string Name => Type.Name;

		public static SystemType SystemType => Type.SystemType;

		public static IImmutableList<ConstructorMember> Constructors => MemberCache.Constructors[Handle];

		public static IImmutableDictionary<string, FieldMember> Fields => MemberCache.Fields[Handle];

		public static IImmutableList<IndexerMember> Indexers => MemberCache.Indexers[Handle];

		public static IImmutableDictionary<string, IImmutableList<MethodMember>> Methods => MemberCache.Methods[Handle];

		public static IImmutableDictionary<string, PropertyMember> Properties => MemberCache.Properties[Handle];

		public static IImmutableDictionary<string, StaticFieldMember> StaticFields => MemberCache.StaticFields[Handle];

		public static IImmutableDictionary<string, IImmutableList<StaticMethodMember>> StaticMethods => MemberCache.StaticMethods[Handle];

		public static IImmutableDictionary<string, StaticPropertyMember> StaticProperties => MemberCache.StaticProperties[Handle];

		public static T Create(params object[] parameters)
			=> (T?)Constructors.First(constructor => constructor!.IsCallableWith(parameters))?.Create!(parameters)
				?? throw new ArgumentException($"TypeOf<{Name}>.{nameof(Create)}: failed with {parameters?.Length ?? 0} parameters.");

		public static D? GetMethod<D>(string name)
			where D : Delegate
		{
			name.AssertNotBlank(nameof(name));

			return Methods.Get(name).To(_ => _.Method).First<D>();
		}

		public static D? GetStaticMethod<D>(string name)
			where D : Delegate
		{
			name.AssertNotBlank(nameof(name));

			return StaticMethods.Get(name).To(_ => _.Method).First<D>();
		}
	}
}
