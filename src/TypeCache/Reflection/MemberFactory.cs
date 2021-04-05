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
			=> typeHandle.ToType().GetConstructors(INSTANCE_BINDINGS)
				.If(constructorInfo => !constructorInfo!.GetParameters().Any(_ => _!.ParameterType.IsPointer || _.ParameterType.IsByRefLike))!
				.To(constructorInfo => (constructorInfo!).ToMember())
				.ToImmutableArray();

		public static IImmutableDictionary<string, EventMember> CreateEventMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetEvents(INSTANCE_BINDINGS)
				.To(eventInfo => KeyValuePair.Create(eventInfo!.Name, eventInfo.ToMember()))
				.ToImmutableDictionary(StringComparison.Ordinal);

		public static IImmutableDictionary<string, FieldMember> CreateFieldMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetFields(INSTANCE_BINDINGS)
				.If(fieldInfo => !fieldInfo!.IsLiteral && !fieldInfo.FieldType.IsByRefLike)!
				.To(fieldInfo => KeyValuePair.Create(fieldInfo!.Name, fieldInfo.ToMember()))
				.ToImmutableDictionary(StringComparison.Ordinal);

		public static IImmutableList<IndexerMember> CreateIndexerMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetProperties(INSTANCE_BINDINGS)
				.If(propertyInfo => propertyInfo!.GetIndexParameters().Any())!
				.To(propertyInfo => (propertyInfo!).ToIndexerMember())
				.ToImmutableArray();

		public static MethodMember CreateMethodMember(RuntimeTypeHandle delegateTypeHandle)
			=> delegateTypeHandle.ToType().GetMethod("Invoke", INSTANCE_BINDINGS)!.ToMember();

		public static IImmutableDictionary<string, IImmutableList<MethodMember>> CreateMethodMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetMethods(INSTANCE_BINDINGS)
				.If(methodInfo => !methodInfo!.ContainsGenericParameters && !methodInfo.IsSpecialName)
				.To(methodInfo => methodInfo!.ToMember())
				.Group(method => method!.Name, StringComparer.Ordinal)
				.ToImmutableDictionary(_ => _.Key, _ => (IImmutableList<MethodMember>)_.Value.ToImmutableArray(), StringComparison.Ordinal);

		public static IImmutableDictionary<string, PropertyMember> CreatePropertyMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetProperties(INSTANCE_BINDINGS)
				.If(propertyInfo => !propertyInfo!.GetIndexParameters().Any())!
				.To(propertyInfo => KeyValuePair.Create(propertyInfo!.Name, propertyInfo.ToMember()))
				.ToImmutableDictionary(StringComparison.Ordinal);

		public static IImmutableDictionary<string, StaticEventMember> CreateStaticEventMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetEvents(STATIC_BINDINGS)
				.To(eventInfo => KeyValuePair.Create(eventInfo!.Name, eventInfo.ToStaticMember()))
				.ToImmutableDictionary(StringComparison.Ordinal);

		public static IImmutableDictionary<string, StaticFieldMember> CreateStaticFieldMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetFields(STATIC_BINDINGS)
				.If(fieldInfo => !fieldInfo!.FieldType.IsByRefLike)!
				.To(fieldInfo => KeyValuePair.Create(fieldInfo!.Name, fieldInfo.ToStaticMember()))
				.ToImmutableDictionary(StringComparison.Ordinal);

		public static IImmutableDictionary<string, IImmutableList<StaticMethodMember>> CreateStaticMethodMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetMethods(STATIC_BINDINGS)
				.If(methodInfo => !methodInfo!.ContainsGenericParameters && !methodInfo.IsSpecialName)
				.To(methodInfo => methodInfo!.ToStaticMember())
				.Group(method => method!.Name, StringComparer.Ordinal)
				.ToImmutableDictionary(_ => _.Key, _ => (IImmutableList<StaticMethodMember>)_.Value.ToImmutableArray(), StringComparison.Ordinal);

		public static IImmutableDictionary<string, StaticPropertyMember> CreateStaticPropertyMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetProperties(STATIC_BINDINGS)
				.To(propertyInfo => KeyValuePair.Create(propertyInfo!.Name, propertyInfo.ToStaticMember()))
				.ToImmutableDictionary(StringComparison.Ordinal);
	}
}
