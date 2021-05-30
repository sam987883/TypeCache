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
		private static readonly TypeMember TypeMember = MemberCache.Types[typeof(T).TypeHandle];

		public static IImmutableList<Attribute> Attributes => TypeMember.Attributes;

		public static RuntimeTypeHandle BaseTypeHandle => TypeMember.BaseTypeHandle;

		public static TypeMember BaseType { get; } = MemberCache.Types[TypeMember.BaseTypeHandle];

		public static TypeMember? EnclosedType { get; } = TypeMember.EnclosedTypeHandle.HasValue ? MemberCache.Types[TypeMember.EnclosedTypeHandle.Value] : null;

		public static IImmutableList<TypeMember> GenericTypes { get; } = TypeMember.GenericTypeHandles.To(handle => MemberCache.Types[handle]).ToImmutableArray();

		public static RuntimeTypeHandle Handle => TypeMember.Handle;

		public static IImmutableList<TypeMember> InterfaceTypes { get; } = TypeMember.InterfaceTypeHandles.To(handle => MemberCache.Types[handle]).ToImmutableArray();

		public static bool Is<V>() => TypeMember.Handle.Is<V>();

		public static bool Is(Type type) => TypeMember.Handle.Is(type);

		public static bool IsEnumerable => TypeMember.IsEnumerable;

		public static bool IsInternal => TypeMember.IsInternal;

		public static bool IsPublic => TypeMember.IsPublic;

		public static Kind Kind => TypeMember.Kind;

		public static string Name => TypeMember.Name;

		public static SystemType SystemType => TypeMember.SystemType;

		public static IImmutableList<ConstructorMember> Constructors => MemberCache.Constructors[Handle];

		public static IImmutableDictionary<string, InstanceFieldMember> Fields => MemberCache.Fields[Handle];

		public static IImmutableList<IndexerMember> Indexers => MemberCache.Indexers[Handle];

		public static IImmutableDictionary<string, IImmutableList<InstanceMethodMember>> Methods => MemberCache.Methods[Handle];

		public static IImmutableDictionary<string, InstancePropertyMember> Properties => MemberCache.Properties[Handle];

		public static IImmutableDictionary<string, StaticFieldMember> StaticFields => MemberCache.StaticFields[Handle];

		public static IImmutableDictionary<string, IImmutableList<StaticMethodMember>> StaticMethods => MemberCache.StaticMethods[Handle];

		public static IImmutableDictionary<string, StaticPropertyMember> StaticProperties => MemberCache.StaticProperties[Handle];

		public static T Create(params object?[]? parameters)
		{
			var constructor = Constructors.First(constructor => constructor!.IsCallableWith(parameters));
			if (constructor != null)
				return (T)constructor.Create!(parameters);
			throw new ArgumentException($"{nameof(TypeOf<T>)}<{Name}>.{nameof(Create)}(...): no constructor found that takes the {parameters?.Length ?? 0} provided {nameof(parameters)}.");
		}

		public static D? GetMethod<D>(string name)
			where D : Delegate
			=> Methods.Get(name).To(_ => _.Method).First<D>();

		public static D? GetStaticMethod<D>(string name)
			where D : Delegate
			=> StaticMethods.Get(name).To(_ => _.Method).First<D>();
	}
}
