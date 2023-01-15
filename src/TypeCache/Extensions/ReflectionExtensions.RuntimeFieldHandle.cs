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
}
