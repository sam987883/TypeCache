// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Attributes;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

[DebuggerDisplay("Parameter: {Name,nq}(,,,)", Name = "{Name}")]
public readonly struct MethodParameter : IEquatable<MethodParameter>
{
	private readonly RuntimeMethodHandle _MethodHandle;

	internal MethodParameter(RuntimeMethodHandle methodHandle, ParameterInfo parameterInfo)
	{
		this._MethodHandle = methodHandle;
		this.Attributes = parameterInfo.GetCustomAttributes<Attribute>(true)?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
		this.DefaultValue = parameterInfo.DefaultValue;
		this.HasDefaultValue = parameterInfo.HasDefaultValue;
		this.IsOptional = parameterInfo.IsOptional;
		this.IsOut = parameterInfo.IsOut;
		this.Name = this.Attributes.FirstOrDefault<NameAttribute>()?.Name ?? parameterInfo.Name ?? string.Empty;
		this.TypeHandle = parameterInfo.ParameterType.TypeHandle;
	}

	public IReadOnlyList<Attribute> Attributes { get; }

	public object? DefaultValue { get; }

	public bool HasDefaultValue { get; }

	public bool IsOptional { get; }

	public bool IsOut { get; }

	public string Name { get; }

	public TypeMember Type => this.TypeHandle.GetTypeMember();

	public RuntimeTypeHandle TypeHandle { get; }

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Equals(MethodParameter other)
		=> this._MethodHandle == other._MethodHandle && this.Name.Is(other.Name);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> item is MethodParameter parameter && this.Equals(parameter);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> (this.Name, this._MethodHandle).GetHashCode();
}
