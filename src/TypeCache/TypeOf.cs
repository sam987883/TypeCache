// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;

namespace TypeCache
{
	public static class TypeOf<T>
	{
		public static TypeMember Member { get; } = typeof(T).GetTypeMember();

		public static IImmutableList<Attribute> Attributes => Member.Attributes;

		public static TypeMember BaseType => Member.BaseType;

		public static TypeMember? EnclosedType => Member.EnclosedType;

		public static IImmutableList<TypeMember> GenericTypes => Member.GenericTypes;

		public static RuntimeTypeHandle Handle => Member.Handle;

		public static IImmutableList<TypeMember> InterfaceTypes => Member.InterfaceTypes;

		public static bool Internal => Member.Internal;

		public static Kind Kind => Member.Kind;

		public static string Name => Member.Name;

		public static bool Public => Member.Public;

		public static SystemType SystemType => Member.SystemType;

		public static IImmutableList<ConstructorMember> Constructors => Member.Constructors;

		public static IImmutableDictionary<string, FieldMember> Fields => Member.Fields;

		public static IImmutableDictionary<string, IImmutableList<MethodMember>> Methods => Member.Methods;

		public static IImmutableDictionary<string, PropertyMember> Properties => Member.Properties;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Create(params object?[]? parameters)
			=> (T)Member.Create(parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Implements<V>()
			where V : class
			=> Member.Implements<V>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Implements(Type type)
			=> Member.Implements(type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object? Invoke(string name, params object?[]? parameters)
			=> Member.Invoke(name, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is<V>()
			=> Member.Is<V>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is(Type type)
			=> Member.Is(type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static D? GetConstructor<D>()
			where D : Delegate
			=> Member.GetConstructor<D>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static D? GetMethod<D>(string name, bool isStatic = false)
			where D : Delegate
			=> Member.GetMethod<D>(name, isStatic);
	}
}
