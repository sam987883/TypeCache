// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

[DebuggerDisplay("{Type,nq}.{Name,nq}(,,,)", Name = "{Name}")]
public class MethodMember : Member, IEquatable<MethodMember>
{
	private readonly IReadOnlyDictionary<RuntimeTypeHandle[], InvokeType>? _Cache;

	private readonly InvokeType? _Invoke;

	internal MethodMember(MethodInfo methodInfo, TypeMember type) : base(methodInfo)
	{
		this.Abstract = methodInfo.IsAbstract;
		this.GenericTypes = methodInfo.GetGenericArguments().Length;
		this.Handle = methodInfo.MethodHandle;
		this.Method = !methodInfo.ContainsGenericParameters ? methodInfo.Lambda().Compile() : null;
		this.Parameters = methodInfo.GetParameters().Map(parameter => new MethodParameter(methodInfo.MethodHandle, parameter)).ToImmutableArray();
		this.Static = methodInfo.IsStatic;
		this.Return = new ReturnParameter(methodInfo);

		this._Cache = methodInfo.ContainsGenericParameters ? new LazyDictionary<RuntimeTypeHandle[], InvokeType>(CreateGenericInvoke, comparer: RuntimeTypeHandleArrayComparer) : null;
		this._Invoke = !methodInfo.ContainsGenericParameters ? methodInfo.LambdaInvokeType().Compile() : null;

		InvokeType CreateGenericInvoke(params RuntimeTypeHandle[] handles)
		{
			var types = handles.Map(handle => handle.ToType()).ToArray();
			return methodInfo.MakeGenericMethod(types).LambdaInvokeType().Compile();
		}
	}

	public bool Abstract { get; }

	public int GenericTypes { get; }

	public RuntimeMethodHandle Handle { get; }

	public Delegate? Method { get; }

	public IReadOnlyList<MethodParameter> Parameters { get; }

	public ReturnParameter Return { get; }

	public bool Static { get; }

	/// <summary>
	/// <c>=&gt; <paramref name="other"/> <see langword="is not null"/> &amp;&amp; <see langword="this"/>.Handle.Equals(<paramref name="other"/>.Handle) &amp;&amp; <see langword="this"/>.Type.Equals(<paramref name="other"/>.Type);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Equals(MethodMember? other)
		=> other is not null && this.Handle.Equals(other.Handle) && this.Type!.Equals(other.Type);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> this.Equals(item as MethodMember);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> this.Handle.GetHashCode();

	/// <summary>
	/// <c>=&gt; <see langword="this"/>._Invoke?.Invoke(<paramref name="instance"/>, <paramref name="arguments"/>);</c>
	/// </summary>
	/// <param name="instance">Pass null if the method is static.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public object? Invoke(object? instance, params object?[]? arguments)
		=> this._Invoke?.Invoke(instance, arguments);

	/// <summary>
	/// <c>=&gt; <see langword="this"/>._Cache?[<paramref name="genericTypes"/>.To(type =&gt; type.TypeHandle).ToArray()].Invoke(<paramref name="instance"/>, <paramref name="arguments"/>);</c>
	/// </summary>
	/// <param name="instance">Pass null if the method is static.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public object? InvokeGeneric(object? instance, Type[] genericTypes, params object?[]? arguments)
		=> this._Cache?[genericTypes.Map(type => type.TypeHandle).ToArray()].Invoke(instance, arguments);

	/// <summary>
	/// <c>=&gt; <paramref name="member"/>.Handle.ToMethodBase(<paramref name="member"/>.Type.Handle)!;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static implicit operator MethodInfo(MethodMember member)
		=> (MethodInfo)member.Handle.ToMethodBase(member.Type!.Handle)!;
}
