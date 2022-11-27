// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

[DebuggerDisplay("{Type,nq} Return Parameter")]
public readonly struct ReturnParameter : IEquatable<ReturnParameter>
{
	internal ReturnParameter(MethodInfo methodInfo)
	{
		this._MethodHandle = methodInfo.MethodHandle;
		this.Attributes = methodInfo.ReturnParameter.GetCustomAttributes<Attribute>(true)?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
		this.TypeHandle = methodInfo.ReturnType.TypeHandle;
	}

	private readonly RuntimeMethodHandle _MethodHandle;

	public IImmutableList<Attribute> Attributes { get; }

	public bool Task => this.Type.SystemType is SystemType.Task;

	public TypeMember Type => this.TypeHandle.GetTypeMember();

	public RuntimeTypeHandle TypeHandle { get; }

	public bool ValueTask => this.Type.SystemType is SystemType.ValueTask;

	public bool Void => this.Type.SystemType is SystemType.Void;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Equals(ReturnParameter other)
		=> this._MethodHandle == other._MethodHandle;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> item is ReturnParameter parameter && this.Equals(parameter);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> this._MethodHandle.GetHashCode();
}
