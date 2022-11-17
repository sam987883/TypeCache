// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Reflection;
using static TypeCache.Default;

namespace TypeCache;

public static class EnumOf<T>
	where T : struct, Enum
{
	public static EnumMember<T> Member { get; } = new EnumMember<T>();

	public static IReadOnlyList<Attribute> Attributes => Member.Attributes;

	public static CustomComparer<T> Comparer => Member.Comparer;

	public static bool Flags => Member.Flags;

	public static RuntimeTypeHandle Handle => Member.TypeHandle;

	public static bool Internal => Member.Internal;

	public static string Name => Member.Name;

	public static bool Public => Member.Public;

	public static IReadOnlyCollection<TokenMember<T>> Tokens => Member.Tokens;

	public static TypeMember UnderlyingType => Member.UnderlyingType;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool IsDefined(T value)
		=> Member.IsDefined(value);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string Parse(T value)
		=> Member.Parse(value);
}
