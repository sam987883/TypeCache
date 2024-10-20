// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Utilities;

public static class ValueBox<T>
	where T : struct
{
	private static readonly IReadOnlyDictionary<T, object> BoxedValues = new LazyDictionary<T, object>(value => value);

	public static object GetValue(T value)
		=> BoxedValues[value];
}
