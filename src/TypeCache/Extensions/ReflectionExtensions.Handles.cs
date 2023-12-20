// Copyright (c) 2021 Samuel Abraham

using System.Reflection;

namespace TypeCache.Extensions;

partial class ReflectionExtensions
{
	/// <inheritdoc cref="FieldInfo.GetFieldFromHandle(RuntimeFieldHandle)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="FieldInfo"/>.GetFieldFromHandle(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static FieldInfo ToFieldInfo(this RuntimeFieldHandle @this)
		=> FieldInfo.GetFieldFromHandle(@this);

	/// <inheritdoc cref="FieldInfo.GetFieldFromHandle(RuntimeFieldHandle, RuntimeTypeHandle)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="FieldInfo"/>.GetFieldFromHandle(@<paramref name="this"/>, <paramref name="typeHandle"/>);</c>
	/// </remarks>
	/// <param name="typeHandle"><see cref="RuntimeTypeHandle"/> is needed when <see cref="RuntimeFieldHandle"/> is a field whose type is a generic parameter of its declared type.</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static FieldInfo ToFieldInfo(this RuntimeFieldHandle @this, RuntimeTypeHandle typeHandle)
		=> FieldInfo.GetFieldFromHandle(@this, typeHandle);

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

	/// <inheritdoc cref="Type.GetTypeFromHandle(RuntimeTypeHandle)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Type"/>.GetTypeFromHandle(@<paramref name="this"/>) ?? throw new UnreachableException("Type.GetTypeFromHandle(...) returned null.");</c>
	/// </remarks>
	/// <exception cref="UnreachableException"></exception>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	[return: NotNull]
	public static Type ToType(this RuntimeTypeHandle @this)
		=> Type.GetTypeFromHandle(@this) ?? throw new UnreachableException("Type.GetTypeFromHandle(...) returned null.");
}
