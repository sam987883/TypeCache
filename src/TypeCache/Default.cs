// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TypeCache.Collections;

namespace TypeCache;

public static class Default
{
	public const MethodImplOptions METHOD_IMPL_OPTIONS = MethodImplOptions.AggressiveInlining;

	public const StringComparison STRING_COMPARISON = StringComparison.OrdinalIgnoreCase;
}
