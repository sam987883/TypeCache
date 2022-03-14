// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

public readonly struct ReturnParameter : IEquatable<ReturnParameter>
{
	internal ReturnParameter(MethodInfo methodInfo)
	{
		this._MethodHandle = methodInfo.MethodHandle;
		this.Attributes = methodInfo.ReturnParameter.GetCustomAttributes<Attribute>(true)?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
		this.Type = methodInfo.ReturnType.GetTypeMember();
		this.Void = this.Type.SystemType is SystemType.Void;
		this.Task = this.Type.SystemType is SystemType.Task;
		this.ValueTask = this.Type.SystemType is SystemType.ValueTask;
	}

	private readonly RuntimeMethodHandle _MethodHandle;

	public IImmutableList<Attribute> Attributes { get; }

	public bool Task { get; }

	public TypeMember Type { get; }

	public bool ValueTask { get; }

	public bool Void { get; }

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public bool Equals(ReturnParameter other)
		=> this._MethodHandle == other._MethodHandle;
}
