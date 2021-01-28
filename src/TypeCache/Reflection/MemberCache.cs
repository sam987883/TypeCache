// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection.Members;

namespace TypeCache.Reflection
{
	public static class MemberCache
	{
		public const BindingFlags INSTANCE_BINDINGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		public const BindingFlags STATIC_BINDINGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		static MemberCache()
		{
			Constructors = new LazyDictionary<RuntimeTypeHandle, IImmutableList<IConstructorMember>>(CreateConstructorMembers);
			Fields = new LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IFieldMember>>(CreateFieldMembers);
			Indexers = new LazyDictionary<RuntimeTypeHandle, IImmutableList<IIndexerMember>>(CreateIndexerMembers);
			Methods = new LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IImmutableList<IMethodMember>>>(CreateMethodMembers);
			Properties = new LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IPropertyMember>>(CreatePropertyMembers);
			StaticFields = new LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IStaticFieldMember>>(CreateStaticFieldMembers);
			StaticMethods = new LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IImmutableList<IStaticMethodMember>>>(CreateStaticMethodMembers);
			StaticProperties = new LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IStaticPropertyMember>>(CreateStaticPropertyMembers);
		}

		public static LazyDictionary<RuntimeTypeHandle, IImmutableList<IConstructorMember>> Constructors { get; }

		public static LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IFieldMember>> Fields { get; }

		public static LazyDictionary<RuntimeTypeHandle, IImmutableList<IIndexerMember>> Indexers { get; }

		public static LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IImmutableList<IMethodMember>>> Methods { get; }

		public static LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IPropertyMember>> Properties { get; }

		public static LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IStaticFieldMember>> StaticFields { get; }

		public static LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IImmutableList<IStaticMethodMember>>> StaticMethods { get; }

		public static LazyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IStaticPropertyMember>> StaticProperties { get; }

		public static IImmutableList<IConstructorMember> CreateConstructorMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetConstructors(INSTANCE_BINDINGS)
				.To(constructorInfo => (IConstructorMember)new ConstructorMember(constructorInfo))
				.ToImmutable();

		public static IImmutableDictionary<string, IFieldMember> CreateFieldMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetFields(INSTANCE_BINDINGS)
				.If(fieldInfo => !fieldInfo!.IsLiteral)
				.To(fieldInfo => KeyValuePair.Create(fieldInfo!.Name, (IFieldMember)new FieldMember(fieldInfo)))
				.ToImmutable(StringComparer.Ordinal);

		public static IImmutableList<IIndexerMember> CreateIndexerMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetProperties(INSTANCE_BINDINGS)
				.If(propertyInfo => propertyInfo!.GetIndexParameters().Any())
				.To(propertyInfo => (IIndexerMember)new IndexerMember(propertyInfo!))
				.ToImmutable();

		public static IImmutableDictionary<string, IImmutableList<IMethodMember>> CreateMethodMembers(RuntimeTypeHandle typeHandle)
		{
			var methodInfos = typeHandle.ToType().GetMethods(INSTANCE_BINDINGS)
				.If(methodInfo => !methodInfo!.ContainsGenericParameters && !methodInfo.IsSpecialName)
				.ToArray();
			return methodInfos
				.To(methodInfo => methodInfo!.Name)
				.ToHashSet(StringComparer.Ordinal)
				.To(name => KeyValuePair.Create(name, methodInfos
					.If(methodInfo => StringComparer.Ordinal.Equals(methodInfo!.Name, name))
					.To(methodInfo => (IMethodMember)new MethodMember(methodInfo!))
					.ToImmutable()))
				.ToImmutable(StringComparer.Ordinal);
		}

		public static IImmutableDictionary<string, IPropertyMember> CreatePropertyMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetProperties(INSTANCE_BINDINGS)
				.If(propertyInfo => !propertyInfo!.GetIndexParameters().Any())
				.To(propertyInfo => KeyValuePair.Create(propertyInfo!.Name, (IPropertyMember)new PropertyMember(propertyInfo)))
				.ToImmutable(StringComparer.Ordinal);

		public static IImmutableDictionary<string, IStaticFieldMember> CreateStaticFieldMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetFields(STATIC_BINDINGS)
				.If(fieldInfo => !fieldInfo!.IsLiteral)
				.To(fieldInfo => KeyValuePair.Create(fieldInfo!.Name, (IStaticFieldMember)new StaticFieldMember(fieldInfo)))
				.ToImmutable(StringComparer.Ordinal);

		public static IImmutableDictionary<string, IImmutableList<IStaticMethodMember>> CreateStaticMethodMembers(RuntimeTypeHandle typeHandle)
		{
			var methodInfos = typeHandle.ToType().GetMethods(STATIC_BINDINGS)
				.If(methodInfo => !methodInfo!.ContainsGenericParameters && !methodInfo.IsSpecialName)
				.ToArray();
			return methodInfos
				.To(methodInfo => methodInfo!.Name)
				.ToHashSet(StringComparer.Ordinal)
				.To(name => KeyValuePair.Create(name, methodInfos
					.If(methodInfo => StringComparer.Ordinal.Equals(methodInfo!.Name, name))
					.To(methodInfo => (IStaticMethodMember)new StaticMethodMember(methodInfo!))
					.ToImmutable()))
				.ToImmutable(StringComparer.Ordinal);
		}

		public static IImmutableDictionary<string, IStaticPropertyMember> CreateStaticPropertyMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetProperties(STATIC_BINDINGS)
				.To(propertyInfo => KeyValuePair.Create(propertyInfo.Name, (IStaticPropertyMember)new StaticPropertyMember(propertyInfo)))
				.ToImmutable(StringComparer.Ordinal);
	}
}
