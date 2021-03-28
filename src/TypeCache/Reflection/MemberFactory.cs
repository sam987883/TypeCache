// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection
{
	public static class MemberFactory
	{
		public const BindingFlags INSTANCE_BINDINGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		public const BindingFlags STATIC_BINDINGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		public static IImmutableList<ConstructorMember> CreateConstructorMembers(RuntimeTypeHandle typeHandle)
			=> IEnumerableExtensions.ToImmutableArray(typeHandle.ToType().GetConstructors(INSTANCE_BINDINGS)
				.If(constructorInfo => !constructorInfo!.GetParameters().Any(_ => _!.ParameterType.IsPointer || _.ParameterType.IsByRefLike))!
				.To(constructorInfo => (constructorInfo!).CreateMember()));

		public static IImmutableDictionary<string, EventMember> CreateEventMembers(RuntimeTypeHandle typeHandle)
			=> IEnumerableExtensions.ToImmutableDictionary(typeHandle.ToType().GetEvents(INSTANCE_BINDINGS)
				.To(eventInfo => KeyValuePair.Create(eventInfo!.Name, eventInfo.CreateMember())), StringComparer.Ordinal);

		public static IImmutableDictionary<string, FieldMember> CreateFieldMembers(RuntimeTypeHandle typeHandle)
			=> IEnumerableExtensions.ToImmutableDictionary(typeHandle.ToType().GetFields(INSTANCE_BINDINGS)
				.If(fieldInfo => !fieldInfo!.IsLiteral && !fieldInfo.FieldType.IsByRefLike)!
				.To(fieldInfo => KeyValuePair.Create(fieldInfo!.Name, fieldInfo.CreateMember())), StringComparer.Ordinal);

		public static IImmutableList<IndexerMember> CreateIndexerMembers(RuntimeTypeHandle typeHandle)
			=> IEnumerableExtensions.ToImmutableArray(typeHandle.ToType().GetProperties(INSTANCE_BINDINGS)
				.If(propertyInfo => propertyInfo!.GetIndexParameters().Any())!
				.To(propertyInfo => (propertyInfo!).CreateIndexerMember()));

		public static MethodMember CreateMethodMember(RuntimeTypeHandle delegateTypeHandle)
			=> delegateTypeHandle.ToType().GetMethod("Invoke", INSTANCE_BINDINGS)!.CreateMember();

		public static IImmutableDictionary<string, IImmutableList<MethodMember>> CreateMethodMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetMethods(INSTANCE_BINDINGS)
				.If(methodInfo => !methodInfo!.ContainsGenericParameters && !methodInfo.IsSpecialName)
				.To(methodInfo => methodInfo!.CreateMember())
				.Group(method => method!.Name, StringComparer.Ordinal)
				.ToImmutableDictionary(_ => _.Key, _ => (IImmutableList<MethodMember>)IEnumerableExtensions.ToImmutableArray(_.Value));

		public static IImmutableDictionary<string, PropertyMember> CreatePropertyMembers(RuntimeTypeHandle typeHandle)
			=> IEnumerableExtensions.ToImmutableDictionary(typeHandle.ToType().GetProperties(INSTANCE_BINDINGS)
				.If(propertyInfo => !propertyInfo!.GetIndexParameters().Any())!
				.To(propertyInfo => KeyValuePair.Create(propertyInfo!.Name, propertyInfo.CreateMember())), StringComparer.Ordinal);

		public static IImmutableDictionary<string, StaticEventMember> CreateStaticEventMembers(RuntimeTypeHandle typeHandle)
			=> IEnumerableExtensions.ToImmutableDictionary(typeHandle.ToType().GetEvents(STATIC_BINDINGS)
				.To(eventInfo => KeyValuePair.Create(eventInfo!.Name, eventInfo.CreateStaticMember())), StringComparer.Ordinal);

		public static IImmutableDictionary<string, StaticFieldMember> CreateStaticFieldMembers(RuntimeTypeHandle typeHandle)
			=> IEnumerableExtensions.ToImmutableDictionary(typeHandle.ToType().GetFields(STATIC_BINDINGS)
				.If(fieldInfo => !fieldInfo!.FieldType.IsByRefLike)!
				.To(fieldInfo => KeyValuePair.Create(fieldInfo!.Name, fieldInfo.CreateStaticMember())), StringComparer.Ordinal);

		public static IImmutableDictionary<string, IImmutableList<StaticMethodMember>> CreateStaticMethodMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetMethods(STATIC_BINDINGS)
				.If(methodInfo => !methodInfo!.ContainsGenericParameters && !methodInfo.IsSpecialName)
				.To(methodInfo => methodInfo!.CreateStaticMember())
				.Group(method => method!.Name, StringComparer.Ordinal)
				.ToImmutableDictionary(_ => _.Key, _ => (IImmutableList<StaticMethodMember>)IEnumerableExtensions.ToImmutableArray(_.Value));

		public static IImmutableDictionary<string, StaticPropertyMember> CreateStaticPropertyMembers(RuntimeTypeHandle typeHandle)
			=> IEnumerableExtensions.ToImmutableDictionary(typeHandle.ToType().GetProperties(STATIC_BINDINGS)
				.To(propertyInfo => KeyValuePair.Create(propertyInfo!.Name, propertyInfo.CreateStaticMember())), StringComparer.Ordinal);
	}
}
