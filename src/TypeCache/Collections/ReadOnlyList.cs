// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Collections;

public class ReadOnlyList<T>(IReadOnlyList<T> list) : ReadOnlyCollection<T>(list), IReadOnlyList<T>
{
	public T this[int index] => list[index];
}
