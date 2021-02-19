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
		static TypeOf()
		{
			var type = typeof(T);
			var attributes = type.GetCustomAttributes(true);

			Attributes = attributes.As<Attribute>().ToImmutable(attributes.Length)!;
			CollectionType = type.ToCollectionType();
			IsAsyncDisposable = type.Implements<IAsyncDisposable>();
			IsClass = type.IsClass;
			IsDisposable = type.Implements<IDisposable>();
			IsInternal = !type.IsVisible;
			IsNullable = type.Is(typeof(Nullable<>));
			IsPublic = type.IsPublic;
			IsTask = type.IsTask();
			IsValueTask = type.IsValueTask();
			IsValueType = type.IsValueType;
			Name = Attributes.First<Attribute, NameAttribute>()?.Name ?? type.Name;
			TypeHandle = type.TypeHandle;
		}

		public static IImmutableList<Attribute> Attributes { get; }

		public static CollectionType CollectionType { get; }

		public static bool IsAsyncDisposable { get; }

		public static bool IsClass { get; }

		public static bool IsDisposable { get; }

		public static bool IsInternal { get; }

		public static bool IsNullable { get; }

		public static bool IsPublic { get; }

		public static bool IsTask { get; }

		public static bool IsValueTask { get; }

		public static bool IsValueType { get; }

		public static string Name { get; }

		public static RuntimeTypeHandle TypeHandle { get; }

		public static IImmutableList<ConstructorMember> Constructors => MemberCache.Constructors[TypeHandle];

		public static IImmutableDictionary<string, FieldMember> Fields => MemberCache.Fields[TypeHandle];

		public static IImmutableList<IndexerMember> Indexers => MemberCache.Indexers[TypeHandle];

		public static IImmutableDictionary<string, IImmutableList<MethodMember>> Methods => MemberCache.Methods[TypeHandle];

		public static IImmutableDictionary<string, PropertyMember> Properties => MemberCache.Properties[TypeHandle];

		public static IImmutableDictionary<string, StaticFieldMember> StaticFields => MemberCache.StaticFields[TypeHandle];

		public static IImmutableDictionary<string, IImmutableList<StaticMethodMember>> StaticMethods => MemberCache.StaticMethods[TypeHandle];

		public static IImmutableDictionary<string, StaticPropertyMember> StaticProperties => MemberCache.StaticProperties[TypeHandle];

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

			return Methods.Get(name).To(_ => _.Method).If<Delegate, D>().First();
		}

		public static D? GetStaticMethod<D>(string name)
			where D : Delegate
		{
			name.AssertNotBlank(nameof(name));

			return StaticMethods.Get(name).To(staticMethodMember => staticMethodMember.Method).First<Delegate, D>();
		}
	}
}
