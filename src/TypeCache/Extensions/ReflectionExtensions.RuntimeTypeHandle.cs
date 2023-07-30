// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

partial class ReflectionExtensions
{
	/// <inheritdoc cref="RuntimeTypeHandle.Equals(RuntimeTypeHandle)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Equals(<see langword="typeof"/>(<typeparamref name="T"/>).TypeHandle);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Is<T>(this RuntimeTypeHandle @this)
		=> @this.Equals(typeof(T).TypeHandle);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Equals(<paramref name="type"/>.TypeHandle)
	/// || (<paramref name="type"/>.IsGenericTypeDefinition &amp;&amp; @<paramref name="this"/>.ToType().ToGenericType() == <paramref name="type"/>);</c>
	/// </remarks>
	[DebuggerHidden]
	public static bool Is(this RuntimeTypeHandle @this, Type type)
		=> @this.Equals(type.TypeHandle) || type.IsGenericTypeDefinition && @this.ToType().ToGenericTypeDefinition() == type;

	/// <inheritdoc cref="Type.MakeArrayType(int)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToType().MakeArrayType(<paramref name="rank"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToArrayTypeOf(this RuntimeTypeHandle @this, int rank = 1)
		=> @this.ToType().MakeArrayType(rank);

	/// <inheritdoc cref="Type.MakeGenericType(Type[])"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToType().MakeGenericType(<paramref name="typeArguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type ToGenericTypeOf(this RuntimeTypeHandle @this, params Type[] typeArguments)
		=> @this.ToType().MakeGenericType(typeArguments);

	/// <inheritdoc cref="Type.GetTypeFromHandle(RuntimeTypeHandle)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Type"/>.GetTypeFromHandle(@<paramref name="this"/>);</c>
	/// </remarks>
	/// <exception cref="UnreachableException"></exception>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	[return: NotNull]
	public static Type ToType(this RuntimeTypeHandle @this)
		=> Type.GetTypeFromHandle(@this) ?? throw new UnreachableException("Type.GetTypeFromHandle(...) returned null.");
}
