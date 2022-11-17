// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;

namespace TypeCache;

public static class Default
{
	public const BindingFlags BINDING_FLAGS = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

	public const string DATASOURCE = "Default";

	public const char GENERIC_TICKMARK = '`';

	public const BindingFlags INSTANCE_BINDING_FLAGS = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

	public const MethodImplOptions METHOD_IMPL_OPTIONS = MethodImplOptions.AggressiveInlining;

	public const StringComparison NAME_STRING_COMPARISON = StringComparison.Ordinal;

	public const RegexOptions REGEX_OPTIONS = RegexOptions.Compiled | RegexOptions.CultureInvariant;

	public const StringComparison SORT_STRING_COMPARISON = StringComparison.Ordinal;

	public const BindingFlags STATIC_BINDING_FLAGS = BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

	public const StringComparison STRING_COMPARISON = StringComparison.OrdinalIgnoreCase;

	public static readonly ConstantExpression NullExpression = Expression.Constant(null);

	public static readonly IComparer<ParameterInfo> ParameterPositionComparer = Comparer<ParameterInfo>.Create((x, y) => x.Position - y.Position);

	public static readonly TimeSpan RegexTimeout = TimeSpan.FromMinutes(1);

	public static readonly IEqualityComparer<RuntimeTypeHandle[]> RuntimeTypeHandleArrayComparer = new CustomEqualityComparer<RuntimeTypeHandle[]>((a, b) => a.IsSequence(b));
}
