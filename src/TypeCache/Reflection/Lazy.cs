// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using static TypeCache.Default;

namespace TypeCache.Reflection;

public static class Lazy
{
	/// <summary>
	/// <c>=&gt; <see langword="new"/> <see cref="Lazy{T}"/>(<paramref name="factory"/>, <paramref name="mode"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Lazy<T> Create<T>(Func<T> factory, LazyThreadSafetyMode mode = LazyThreadSafetyMode.PublicationOnly)
		=> new Lazy<T>(factory, mode);

	/// <summary>
	/// <c>=&gt; <see langword="new"/> <see cref="Lazy{T, TMetadata}"/>(<paramref name="factory"/>, <paramref name="mode"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Lazy<T, M> Create<T, M>(Func<T> factory, M metadata, LazyThreadSafetyMode mode = LazyThreadSafetyMode.PublicationOnly)
		=> new Lazy<T, M>(factory, metadata, mode);

	/// <summary>
	/// <c>=&gt; <see langword="new"/> <see cref="Lazy{T}"/>(() =&gt; <see langword="null"/>, <paramref name="mode"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Lazy<T?> Null<T>(LazyThreadSafetyMode mode = LazyThreadSafetyMode.None)
		where T : class
		=> new Lazy<T?>(() => null, mode);

	/// <summary>
	/// <c>=&gt; <see langword="new"/> <see cref="Lazy{T, TMetadata}"/>(() =&gt; <see langword="null"/>, <paramref name="metadata"/>, <paramref name="mode"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Lazy<T?> Null<T, M>(M metadata, LazyThreadSafetyMode mode = LazyThreadSafetyMode.None)
		where T : class
		=> new Lazy<T?, M>(() => null, metadata, mode);

	/// <summary>
	/// <c>=&gt; <see langword="new"/> <see cref="Lazy{T}"/>(() =&gt; <paramref name="value"/>, <paramref name="mode"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Lazy<T> Value<T>(T value, LazyThreadSafetyMode mode = LazyThreadSafetyMode.None)
		=> new Lazy<T>(() => value, mode);

	/// <summary>
	/// <c>=&gt; <see langword="new"/> <see cref="Lazy{T}"/>(() =&gt; <paramref name="value"/>, <paramref name="metadata"/>, <paramref name="mode"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static Lazy<T, M> Value<T, M>(T value, M metadata, LazyThreadSafetyMode mode = LazyThreadSafetyMode.None)
		=> new Lazy<T, M>(() => value, metadata, mode);
}
