﻿// Copyright (c) 2021 Samuel Abraham

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
public sealed class MethodMember : IMember, IEquatable<MethodMember>
{
	private readonly IReadOnlyDictionary<RuntimeTypeHandle[], Func<object?[]?, object>> _GenericInvokeCache;
	private readonly Lazy<Func<object?[]?, object?>> _Invoke;
	private readonly Lazy<Delegate> _Method;

	internal MethodMember(MethodInfo methodInfo, TypeMember type)
	{
		this.Abstract = methodInfo.IsAbstract;
		this.Attributes = methodInfo.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
		this.GenericTypeCount = methodInfo.GetGenericArguments().Length;
		this.Handle = methodInfo.MethodHandle;
		this.Internal = methodInfo.IsAssembly;
		this.Name = methodInfo.Name();
		this.Parameters = methodInfo.GetParameters().Map(parameter => new MethodParameter(methodInfo.MethodHandle, parameter)).ToImmutableArray();
		this.Public = methodInfo.IsPublic;
		this.Return = new ReturnParameter(methodInfo);
		this.Static = methodInfo.IsStatic;
		this.Type = type;

		this._GenericInvokeCache = methodInfo.ContainsGenericParameters
			? new LazyDictionary<RuntimeTypeHandle[], Func<object?[]?, object>>(createGenericInvoke, comparer: RuntimeTypeHandleArrayComparer)
			: ImmutableDictionary<RuntimeTypeHandle[], Func<object?[]?, object>>.Empty;
		this._Invoke = Lazy.Create(() => ((MethodInfo)this).LambdaInvoke().Compile());
		this._Method = Lazy.Create(() => ((MethodInfo)this).Lambda().Compile());

		Func<object?[]?, object> createGenericInvoke(params RuntimeTypeHandle[] handles)
		{
			var types = handles.Map(handle => handle.ToType()).ToArray();
			return methodInfo.MakeGenericMethod(types).LambdaInvoke().Compile()!;
		}
	}

	/// <inheritdoc cref="MethodBase.IsAbstract"/>
	public bool Abstract { get; }

	/// <inheritdoc/>
	public IReadOnlyList<Attribute> Attributes { get; }

	public int GenericTypeCount { get; }

	/// <inheritdoc cref="MethodBase.MethodHandle"/>
	public RuntimeMethodHandle Handle { get; }

	/// <inheritdoc cref="MethodBase.IsAssembly"/>
	public bool Internal { get; }

	public Delegate? Method => this._Method.Value;

	/// <inheritdoc/>
	public string Name { get; }

	public IReadOnlyList<MethodParameter> Parameters { get; }

	/// <inheritdoc cref="MethodBase.IsPublic"/>
	public bool Public { get; }

	public ReturnParameter Return { get; }

	/// <inheritdoc cref="MethodBase.IsStatic"/>
	public bool Static { get; }

	/// <summary>
	/// The <see cref="TypeMember"/> that owns this method.
	/// </summary>
	public TypeMember Type { get; }

	/// <summary>
	/// <c>=&gt; <paramref name="other"/> <see langword="is not null"/> &amp;&amp; <see langword="this"/>.Handle.Equals(<paramref name="other"/>.Handle) &amp;&amp; <see langword="this"/>.Type.Equals(<paramref name="other"/>.Type);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Equals(MethodMember? other)
		=> this.Handle == other?.Handle;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> this.Equals(item as MethodMember);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> this.Handle.GetHashCode();

	/// <summary>
	/// <c>=&gt; <see langword="this"/>._Invoke.Value(<paramref name="arguments"/>);</c>
	/// </summary>
	/// <remarks>First item in <paramref name="arguments"/> must be the instance of the type that the methode belongs to, unless the method is <c><see langword="static"/></c>.</remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public object? Invoke(params object?[]? arguments)
		=> this._Invoke.Value(arguments);

	/// <summary>
	/// <c>=&gt; <see langword="this"/>._Cache?[<paramref name="genericTypes"/>.Map(type =&gt; type.TypeHandle).ToArray()].Invoke(<paramref name="arguments"/>);</c>
	/// </summary>
	/// <remarks>First item in <paramref name="arguments"/> must be the instance of the type that the methode belongs to, unless the method is <c><see langword="static"/></c>.</remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public object? InvokeGeneric(Type[] genericTypes, params object?[]? arguments)
		=> this._GenericInvokeCache[genericTypes.Map(type => type.TypeHandle).ToArray()](arguments);

	/// <summary>
	/// <c>=&gt; <paramref name="member"/>.Handle.ToMethodBase(<paramref name="member"/>.Type.Handle)!;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static implicit operator MethodInfo(MethodMember member)
		=> (MethodInfo)member.Handle.ToMethodBase(member.Type.TypeHandle)!;
}
