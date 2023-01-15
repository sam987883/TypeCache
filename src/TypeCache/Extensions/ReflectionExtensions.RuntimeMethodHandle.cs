// Copyright (c) 2021 Samuel Abraham

using System.Reflection;

namespace TypeCache.Extensions;

partial class ReflectionExtensions
{
	/// <inheritdoc cref="MethodBase.GetMethodFromHandle(RuntimeMethodHandle)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MethodBase"/>.GetMethodFromHandle(@<paramref name="this"/>);</c>
	/// </remarks>
	/// <exception cref="MissingMethodException"></exception>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodBase ToMethodBase(this RuntimeMethodHandle @this)
		=> MethodBase.GetMethodFromHandle(@this) ?? throw new MissingMethodException("MethodBase.GetMethodFromHandle(...) returned null, method may depend on class generic parameter.");

	/// <inheritdoc cref="MethodBase.GetMethodFromHandle(RuntimeMethodHandle, RuntimeTypeHandle)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MethodBase"/>.GetMethodFromHandle(@<paramref name="this"/>, <paramref name="typeHandle"/>);</c>
	/// </remarks>
	/// <param name="typeHandle"><see cref="RuntimeTypeHandle"/> is needed when <see cref="RuntimeMethodHandle"/> is a method using a generic parameter of its declared type.</param>
	/// <exception cref="UnreachableException"></exception>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodBase ToMethodBase(this RuntimeMethodHandle @this, RuntimeTypeHandle typeHandle)
		=> MethodBase.GetMethodFromHandle(@this, typeHandle) ?? throw new UnreachableException("MethodBase.GetMethodFromHandle(..., ...) returned null.");
}
