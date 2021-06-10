// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;

namespace TypeCache
{
	public static class TypeOf<T>
	{
		public static TypeMember Member { get; } = typeof(T).GetTypeMember();

		public static IImmutableList<Attribute> Attributes => Member.Attributes;

		public static RuntimeTypeHandle BaseTypeHandle => Member.BaseTypeHandle;

		public static TypeMember BaseType { get; } = Member.BaseTypeHandle.GetTypeMember();

		public static TypeMember? EnclosedType { get; } = Member.EnclosedTypeHandle?.GetTypeMember();

		public static IImmutableList<TypeMember> GenericTypes { get; } = Member.GenericTypeHandles.To(handle => handle.GetTypeMember()).ToImmutableArray();

		public static RuntimeTypeHandle Handle => Member.Handle;

		public static IImmutableList<TypeMember> InterfaceTypes { get; } = Member.InterfaceTypeHandles.To(handle => handle.GetTypeMember()).ToImmutableArray();

		public static bool Is<V>() => Member.Handle.Is<V>();

		public static bool Is(Type type) => Member.Handle.Is(type);

		public static bool IsEnumerable => Member.IsEnumerable;

		public static bool IsInternal => Member.Internal;

		public static bool IsPublic => Member.Public;

		public static Kind Kind => Member.Kind;

		public static string Name => Member.Name;

		public static SystemType SystemType => Member.SystemType;

		public static IImmutableList<ConstructorMember> Constructors => Member.Constructors;

		public static IImmutableDictionary<string, FieldMember> Fields => Member.Fields;

		public static IImmutableDictionary<string, IImmutableList<MethodMember>> Methods => Member.Methods;

		public static IImmutableDictionary<string, PropertyMember> Properties => Member.Properties;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Create(params object?[]? parameters)
			=> (T)Member.Create(parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static D? GetMethod<D>(string name, bool isStatic = false)
			where D : Delegate
			=> Member.GetMethod<D>(name, isStatic);
	}
}
