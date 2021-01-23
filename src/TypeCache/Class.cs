// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Cache;

namespace TypeCache
{
	public static class Class<T>
		where T : class
	{
		static Class()
		{
			var type = typeof(T);
			var attributes = type.GetCustomAttributes(true);

			Attributes = attributes.As<Attribute>().ToImmutable(attributes.Length);
			CollectionType = type.ToCollectionType();
			IsAsyncDisposable = type.Implements<IAsyncDisposable>();
			IsDisposable = type.Implements<IDisposable>();
			IsInternal = !type.IsVisible;
			IsPublic = type.IsPublic;
			Name = Attributes.First<Attribute, NameAttribute>()?.Name ?? type.Name;
			TypeHandle = type.TypeHandle;
		}

		public static IImmutableList<Attribute> Attributes { get; }

		public static CollectionType CollectionType { get; }

		public static bool IsAsyncDisposable { get; }

		public static bool IsDisposable { get; }

		public static bool IsInternal { get; }

		public static bool IsPublic { get; }

		public static string Name { get; }

		public static RuntimeTypeHandle TypeHandle { get; }

		public static IImmutableList<IConstructorMember> Constructors => ConstructorCache<T>.Constructors;

		public static IImmutableDictionary<string, IFieldMember> Fields => FieldCache<T>.Fields;

		public static IImmutableList<IIndexerMember> Indexers => IndexerCache<T>.Indexers;

		public static IImmutableDictionary<string, IImmutableList<IMethodMember>> Methods => MethodCache<T>.Methods;

		public static IImmutableDictionary<string, IPropertyMember> Properties => PropertyCache<T>.Properties;

		public static IImmutableDictionary<string, IStaticFieldMember> StaticFields => StaticFieldCache<T>.Fields;

		public static IImmutableDictionary<string, IImmutableList<IStaticMethodMember>> StaticMethods => StaticMethodCache<T>.Methods;

		public static IImmutableDictionary<string, IStaticPropertyMember> StaticProperties => StaticPropertyCache<T>.Properties;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Create()
			=> Constructors.First(constructor => !constructor.Parameters.Any())?.Invoke(null) as T
				?? throw new ArgumentException($"Create instance of class {Name} failed with 0 parameters.");

		public static T Create(params object[] parameters)
		{
			var constructor = Constructors.First(constructor => constructor.IsCallableWith(parameters));
			return constructor?.Invoke(parameters) as T ?? throw new ArgumentException($"Create instance of class {Name} failed with {parameters?.Length ?? 0} parameters.");
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
