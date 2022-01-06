// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection;

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
		this.Name = this.Attributes.First<NameAttribute>()?.Name ?? parameterInfo.Name ?? string.Empty;
		this.Type = parameterInfo.ParameterType.GetTypeMember();
	}

	public IImmutableList<Attribute> Attributes { get; }

	public object? DefaultValue { get; }

	public bool HasDefaultValue { get; }

	public bool IsOptional { get; }

	public bool IsOut { get; }

	public string Name { get; }

	public TypeMember Type { get; }

	public bool Equals(MethodParameter other)
		=> this._MethodHandle == other._MethodHandle && this.Name.Is(other.Name);
}
