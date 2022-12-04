// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using TypeCache.Reflection;

namespace TypeCache.Extensions;

public static class HandleExtensions
{
	/// <remarks>
	/// <c>=&gt; <see cref="TypeMember.Cache"/>[@<paramref name="this"/>];</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TypeMember GetTypeMember(this RuntimeTypeHandle @this)
		=> TypeMember.Cache[@this];

	/// <inheritdoc cref="RuntimeTypeHandle.Equals(RuntimeTypeHandle)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Equals(<see langword="typeof"/>(<typeparamref name="T"/>).TypeHandle);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Is<T>(this RuntimeTypeHandle @this)
		=> @this.Equals(typeof(T).TypeHandle);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Equals(<paramref name="type"/>.TypeHandle)
	/// || (<paramref name="type"/>.IsGenericTypeDefinition &amp;&amp; @<paramref name="this"/>.ToGenericType() == <paramref name="type"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Is(this RuntimeTypeHandle @this, Type type)
		=> @this.Equals(type.TypeHandle) || type.IsGenericTypeDefinition && @this.ToGenericType() == type;

	/// <inheritdoc cref="Type.MakeArrayType(int)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToType().MakeArrayType(<paramref name="rank"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type? ToArrayTypeOf(this RuntimeTypeHandle @this, int rank = 1)
		=> @this.ToType().MakeArrayType(rank);

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

	/// <remarks>
	/// <c>=&gt; @this.ToType() <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    null"/> =&gt; <see langword="null"/>,<br/>
	/// <see langword="    var"/> type <see langword="when"/> type.IsGenericType =&gt; type.GetGenericTypeDefinition(),<br/>
	/// <see langword="    "/>_ =&gt; <see langword="null"/><br/>
	/// };
	/// </c>
	/// </remarks>
	public static Type? ToGenericType(this RuntimeTypeHandle @this)
		=> @this.ToType() switch
		{
			null => null,
			var type when type.IsGenericType => type.GetGenericTypeDefinition(),
			_ => null
		};

	/// <inheritdoc cref="Type.MakeGenericType(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToType().MakeGenericType(<paramref name="typeArguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type? ToGenericTypeOf(this RuntimeTypeHandle @this, params Type[] typeArguments)
		=> @this.ToType().MakeGenericType(typeArguments);

	/// <inheritdoc cref="MethodBase.GetMethodFromHandle(RuntimeMethodHandle)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MethodBase"/>.GetMethodFromHandle(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodBase ToMethodBase(this RuntimeMethodHandle @this)
		=> MethodBase.GetMethodFromHandle(@this) ?? throw new UnreachableException("MethodBase.GetMethodFromHandle(...) returned null.");

	/// <inheritdoc cref="MethodBase.GetMethodFromHandle(RuntimeMethodHandle, RuntimeTypeHandle)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="MethodBase"/>.GetMethodFromHandle(@<paramref name="this"/>, <paramref name="typeHandle"/>);</c>
	/// </remarks>
	/// <param name="typeHandle"><see cref="RuntimeTypeHandle"/> is needed when <see cref="RuntimeMethodHandle"/> is a method using a generic parameter of its declared type.</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodBase ToMethodBase(this RuntimeMethodHandle @this, RuntimeTypeHandle typeHandle)
		=> MethodBase.GetMethodFromHandle(@this, typeHandle) ?? throw new UnreachableException("MethodBase.GetMethodFromHandle(..., ...) returned null.");

	/// <inheritdoc cref="Type.GetTypeFromHandle(RuntimeTypeHandle)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Type"/>.GetTypeFromHandle(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToType(this RuntimeTypeHandle @this)
		=> Type.GetTypeFromHandle(@this) ?? throw new UnreachableException("Type.GetTypeFromHandle(...) returned null.");
}
