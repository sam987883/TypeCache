// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Utilities;

public static class Array<T>
{
	/// <inheritdoc cref="Array.Empty{T}"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Array.Empty{T}"/>;</c>
	/// </remarks>
	public static T[] Empty => Array.Empty<T>();
}
