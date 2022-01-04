// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

public class MethodMember : Member, IEquatable<MethodMember>
{
	private readonly IReadOnlyDictionary<RuntimeTypeHandle[], InvokeType>? _Cache;

	private readonly InvokeType? _Invoke;

	internal MethodMember(MethodInfo methodInfo, TypeMember type) : base(methodInfo)
	{
		this.Type = type;
		this.Abstract = methodInfo.IsAbstract;
		this.GenericTypes = methodInfo.GetGenericArguments().Length;
		this.Handle = methodInfo.MethodHandle;
		this.Method = !methodInfo.ContainsGenericParameters ? methodInfo.Lambda().Compile() : null;
		this.Parameters = methodInfo.GetParameters().To(parameter => new MethodParameter(methodInfo.MethodHandle, parameter)).ToImmutableArray();
		this.Static = methodInfo.IsStatic;
		this.Return = new ReturnParameter(methodInfo);

		this._Cache = methodInfo.ContainsGenericParameters ? new LazyDictionary<RuntimeTypeHandle[], InvokeType>(CreateGenericInvoke, Default.RuntimeTypeHandleArrayComparer) : null;
		this._Invoke = !methodInfo.ContainsGenericParameters ? methodInfo.LambdaInvokeType().Compile() : null;

		InvokeType CreateGenericInvoke(params RuntimeTypeHandle[] handles)
		{
			var types = handles.To(handle => handle.ToType()).ToArray();
			return methodInfo.MakeGenericMethod(types).LambdaInvokeType().Compile();
		}
	}

	public bool Abstract { get; }

	public int GenericTypes { get; }

	public RuntimeMethodHandle Handle { get; }

	public Delegate? Method { get; }

	public IImmutableList<MethodParameter> Parameters { get; }

	public ReturnParameter Return { get; }

	public bool Static { get; }

	public TypeMember Type { get; }

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public bool Equals(MethodMember? other)
		=> this.Handle == other?.Handle && this.Type.Equals(other?.Type);

	/// <param name="instance">Pass null if the method is static.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public object? Invoke(object? instance, params object?[]? arguments)
		=> this._Invoke?.Invoke(instance, arguments);

	/// <param name="instance">Pass null if the method is static.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public object? InvokeGeneric(object? instance, Type[] genericTypes, params object?[]? arguments)
		=> this._Cache?[genericTypes.To(type => type.TypeHandle).ToArray()].Invoke(instance, arguments);

	/// <summary>
	/// <c>=&gt; <paramref name="member"/>.Handle.ToMethodBase(<paramref name="member"/>.Type.Handle)!;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static implicit operator MethodInfo(MethodMember member)
		=> (MethodInfo)member.Handle.ToMethodBase(member.Type.Handle)!;
}
