// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

[DebuggerDisplay("{Type,nq}.ctor", Name = "{Name}")]
public sealed class ConstructorMember : IMember, IEquatable<ConstructorMember>
{
	private readonly Lazy<Func<object?[]?, object?>> _Invoke;
	private readonly Lazy<Delegate> _Method;

	internal ConstructorMember(ConstructorInfo constructorInfo, TypeMember type)
	{
		this.Attributes = constructorInfo.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
		this.Internal = constructorInfo.IsAssembly;
		this.MethodHandle = constructorInfo.MethodHandle;
		this.Name = constructorInfo.Name();
		this.Parameters = constructorInfo.GetParameters().Select(parameter => new MethodParameter(constructorInfo.MethodHandle, parameter)).ToImmutableArray();
		this.Public = constructorInfo.IsPublic;
		this.Type = type;
		this._Invoke = Lazy.Create(() => ((ConstructorInfo)this.MethodHandle.ToMethodBase(this.Type.TypeHandle)!).LambdaInvoke().Compile());
		this._Method = Lazy.Create(() => ((ConstructorInfo)this.MethodHandle.ToMethodBase(this.Type.TypeHandle)!).Lambda().Compile());
	}

	/// <inheritdoc/>
	public IReadOnlyList<Attribute> Attributes { get; }

	/// <inheritdoc cref="MethodBase.MethodHandle"/>
	public RuntimeMethodHandle MethodHandle { get; }

	/// <inheritdoc cref="MethodBase.IsAssembly"/>
	public bool Internal { get; }

	public Delegate? Method => this._Method.Value;

	/// <inheritdoc/>
	public string Name { get; }

	public IReadOnlyList<MethodParameter> Parameters { get; }

	/// <inheritdoc cref="MethodBase.IsPublic"/>
	public bool Public { get; }

	/// <summary>
	/// The <see cref="TypeMember"/> that owns this constructor.
	/// </summary>
	public TypeMember Type { get; }

	/// <summary>
	/// <c>=&gt; <see langword="this"/>._Invoke.Value(<paramref name="arguments"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public object Create(params object?[]? arguments)
		=> this._Invoke.Value(arguments)!;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Equals(ConstructorMember? other)
		=> this.MethodHandle == other?.MethodHandle;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> item is ConstructorMember member && this.Equals(member);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> this.MethodHandle.GetHashCode();

	/// <summary>
	/// <c>=&gt; (<see cref="ConstructorInfo"/>)<paramref name="member"/>.Handle.ToMethodBase(<paramref name="member"/>.Type.Handle)!;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static implicit operator ConstructorInfo(ConstructorMember member)
		=> (ConstructorInfo)member.MethodHandle.ToMethodBase(member.Type!.TypeHandle)!;
}
