// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Collections;

public static class Enumerable<T>
{
	/// <inheritdoc cref="Enumerable.Empty{T}"/>
	/// <remarks>
	/// <c>=&gt; [];</c>
	/// </remarks>
	public static IEnumerable<T> Empty => [];
}
