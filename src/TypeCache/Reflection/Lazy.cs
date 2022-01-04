// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.Reflection;

public static class Lazy
{
	/// <summary>
	/// <c>=&gt; <see langword="new"/> Lazy&lt;T?&gt;(() =&gt; <see langword="null"/>, <see langword="true"/>);</c>
	/// </summary>
	public static Lazy<T?> Null<T>()
		where T : class
		=> new Lazy<T?>(() => null, true);

	public static Lazy<T> Value<T>(T value)
		=> new Lazy<T>(() => value, true);
}
