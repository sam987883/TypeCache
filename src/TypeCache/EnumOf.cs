// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Reflection;
using static TypeCache.Default;

namespace TypeCache;

public static class EnumOf<T>
	where T : struct, Enum
{
	private static readonly EnumMember<T> Member = new EnumMember<T>();

	public static IImmutableList<Attribute> Attributes => Member.Attributes;

	public static CustomComparer<T> Comparer => Member.Comparer;

	public static bool Flags => Member.Flags;

	public static RuntimeTypeHandle Handle => Member.Handle;

	public static bool Internal => Member.Internal;

	public static string Name => Member.Name;

	public static bool Public => Member.Public;

	public static IImmutableDictionary<T, TokenMember<T>> Tokens => Member.Tokens;

	public static TypeMember UnderlyingType => Member.UnderlyingType;

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool IsValid(T value)
		=> Member.IsValid(value);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static string Parse(T value)
		=> Member.Parse(value);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static T? Parse(string text, StringComparison comparison = STRING_COMPARISON)
		=> Member.Parse(text, comparison);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool TryParse(T value, out string text)
		=> Member.TryParse(value, out text);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static bool TryParse(string text, out T value, StringComparison comparison = STRING_COMPARISON)
		=> Member.TryParse(text, out value, comparison);
}
