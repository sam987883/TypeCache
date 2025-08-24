// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;

namespace TypeCache.Utilities;

public static class ValueBox<T>
	where T : struct
{
	private static readonly IReadOnlyDictionary<T, object> BoxedValues = LazyDictionary.Create<T, object>(value => value);

	public static object GetValue(T value)
		=> BoxedValues[value];
}
