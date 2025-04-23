// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Utilities;

public static class Enumerable<T>
{
	/// <inheritdoc cref="Enumerable.Empty{T}"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Enumerable.Empty{T}"/>;</c>
	/// </remarks>
	public static IEnumerable<T> Empty => Enumerable.Empty<T>();
}
