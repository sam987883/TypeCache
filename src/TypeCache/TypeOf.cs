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

		public static CollectionType CollectionType => Type.CollectionType;

		public static RuntimeTypeHandle? BaseHandle => Type.BaseHandle;

		public static RuntimeTypeHandle Handle => Type.Handle;

		public static IImmutableList<RuntimeTypeHandle> InterfaceHandles => Type.InterfaceHandles;

		public static bool IsInternal => Type.IsInternal;

		public static bool IsNullable => Type.IsNullable;

		public static bool IsPublic => Type.IsPublic;

		public static bool IsTask => Type.IsTask;

		public static bool IsValueTask => Type.IsValueTask;

		public static Kind Kind => Type.Kind;

		public static string Name => Type.Name;

		public static IImmutableList<ConstructorMember> Constructors => MemberCache.Constructors[Handle];

		public static IImmutableDictionary<string, FieldMember> Fields => MemberCache.Fields[Handle];

		public static IImmutableList<IndexerMember> Indexers => MemberCache.Indexers[Handle];

		public static IImmutableDictionary<string, IImmutableList<MethodMember>> Methods => MemberCache.Methods[Handle];

		public static IImmutableDictionary<string, PropertyMember> Properties => MemberCache.Properties[Handle];

		public static IImmutableDictionary<string, StaticFieldMember> StaticFields => MemberCache.StaticFields[Handle];

		public static IImmutableDictionary<string, IImmutableList<StaticMethodMember>> StaticMethods => MemberCache.StaticMethods[Handle];

		public static IImmutableDictionary<string, StaticPropertyMember> StaticProperties => MemberCache.StaticProperties[Handle];

		public static T Create()
			=> (T)Constructors.FirstValue(constructor => !constructor!.Parameters.Any())?.Invoke(null)
				?? throw new ArgumentException($"Create instance of class {Name} failed with 0 parameters.");

		public static T Create(params object[] parameters)
		{
			var constructor = Constructors.FirstValue(constructor => constructor!.IsCallableWith(parameters));
			return (T)constructor?.Invoke(parameters) ?? throw new ArgumentException($"Create instance of class {Name} failed with {parameters?.Length ?? 0} parameters.");
		}

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

			return StaticMethods.Get(name).To(staticMethodMember => staticMethodMember.Method).First<D>();
		}
	}
}
