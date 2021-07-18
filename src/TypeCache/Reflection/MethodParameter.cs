// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection
{
	public readonly struct MethodParameter : IEquatable<MethodParameter>
	{
		internal MethodParameter(RuntimeMethodHandle methodHandle, ParameterInfo parameterInfo)
		{
			this._MethodHandle = methodHandle;
			this.Attributes = parameterInfo.GetCustomAttributes<Attribute>(true)?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
			this.DefaultValue = parameterInfo.DefaultValue;
			this.HasDefaultValue = parameterInfo.HasDefaultValue;
			this.IsOptional = parameterInfo.IsOptional;
			this.IsOut = parameterInfo.IsOut;
			this.Name = this.Attributes.First<NameAttribute>()?.Name ?? parameterInfo.Name!;
			this.Type = parameterInfo.ParameterType.GetTypeMember();
		}

		private readonly RuntimeMethodHandle _MethodHandle;

		public IImmutableList<Attribute> Attributes { get; }

		public object? DefaultValue { get; }

		public bool HasDefaultValue { get; }

		public bool IsOptional { get; }

		public bool IsOut { get; }

		public string Name { get; }

		public TypeMember Type { get; }

		public bool Equals(MethodParameter other)
			=> this._MethodHandle == other._MethodHandle && this.Name.Is(other.Name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> HashCode.Combine(this._MethodHandle, this.Name);
	}
}
