// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using TypeCache.Reflection.Members;

namespace TypeCache.Reflection
{
	public static class TypeMemberCache
	{
		public const BindingFlags INSTANCE_BINDINGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		public const BindingFlags STATIC_BINDINGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		static TypeMemberCache()
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

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableList<IConstructorMember>> Constructors { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IFieldMember>> Fields { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableList<IIndexerMember>> Indexers { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IImmutableList<IMethodMember>>> Methods { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IPropertyMember>> Properties { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IStaticFieldMember>> StaticFields { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IImmutableList<IStaticMethodMember>>> StaticMethods { get; }

		public static IReadOnlyDictionary<RuntimeTypeHandle, IImmutableDictionary<string, IStaticPropertyMember>> StaticProperties { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IConstructorMember CreateConstructorMember(ConstructorInfo constructorInfo)
			=> new ConstructorMember(constructorInfo);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IImmutableList<IConstructorMember> CreateConstructorMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetConstructors(INSTANCE_BINDINGS).To(CreateConstructorMember).ToImmutable();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IFieldMember CreateFieldMember(FieldInfo fieldInfo)
			=> new FieldMember(fieldInfo);

		public static IImmutableDictionary<string, IFieldMember> CreateFieldMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetFields(INSTANCE_BINDINGS)
				.If(fieldInfo => !fieldInfo!.IsLiteral)
				.To(fieldInfo => KeyValuePair.Create(fieldInfo!.Name, CreateFieldMember(fieldInfo)))
				.ToImmutable(StringComparer.Ordinal);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IIndexerMember CreateIndexerMember(PropertyInfo propertyInfo)
			=> new IndexerMember(propertyInfo);

		public static IImmutableList<IIndexerMember> CreateIndexerMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetProperties(INSTANCE_BINDINGS)
				.If(propertyInfo => propertyInfo!.GetIndexParameters().Any())
				.To(CreateIndexerMember!)
				.ToImmutable();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IMethodMember CreateMethodMember(MethodInfo methodInfo)
			=> new MethodMember(methodInfo);

		public static IImmutableDictionary<string, IImmutableList<IMethodMember>> CreateMethodMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetMethods(INSTANCE_BINDINGS)
				.If(methodInfo => !methodInfo!.ContainsGenericParameters && !methodInfo.IsSpecialName)
				.To(CreateMethodMember!)
				.Group(method => method!.Name, StringComparer.Ordinal)
				.ToImmutableDictionary(_ => _.Key, _ => _.Value.ToImmutable());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IPropertyMember CreatePropertyMember(PropertyInfo propertyInfo)
			=> new PropertyMember(propertyInfo);

		public static IImmutableDictionary<string, IPropertyMember> CreatePropertyMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetProperties(INSTANCE_BINDINGS)
				.If(propertyInfo => !propertyInfo!.GetIndexParameters().Any())
				.To(propertyInfo => KeyValuePair.Create(propertyInfo!.Name, CreatePropertyMember(propertyInfo)))
				.ToImmutable(StringComparer.Ordinal);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IStaticFieldMember CreateStaticFieldMember(FieldInfo fieldInfo)
			=> new StaticFieldMember(fieldInfo);

		public static IImmutableDictionary<string, IStaticFieldMember> CreateStaticFieldMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetFields(STATIC_BINDINGS)
				.If(fieldInfo => !fieldInfo!.IsLiteral)
				.To(fieldInfo => KeyValuePair.Create(fieldInfo!.Name, CreateStaticFieldMember(fieldInfo)))
				.ToImmutable(StringComparer.Ordinal);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IStaticMethodMember CreateStaticMethodMember(MethodInfo methodInfo)
			=> new StaticMethodMember(methodInfo);

		public static IImmutableDictionary<string, IImmutableList<IStaticMethodMember>> CreateStaticMethodMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetMethods(STATIC_BINDINGS)
				.If(methodInfo => !methodInfo!.ContainsGenericParameters && !methodInfo.IsSpecialName)
				.To(CreateStaticMethodMember!)
				.Group(method => method!.Name, StringComparer.Ordinal)
				.ToImmutableDictionary(_ => _.Key, _ => _.Value.ToImmutable());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IStaticPropertyMember CreateStaticPropertyMember(PropertyInfo propertyInfo)
			=> new StaticPropertyMember(propertyInfo);

		public static IImmutableDictionary<string, IStaticPropertyMember> CreateStaticPropertyMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetProperties(STATIC_BINDINGS)
				.To(propertyInfo => KeyValuePair.Create(propertyInfo.Name, CreateStaticPropertyMember(propertyInfo)))
				.ToImmutable(StringComparer.Ordinal);
	}
}
