// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection
{
	public readonly struct ReturnParameter : IEquatable<ReturnParameter>
	{
		internal ReturnParameter(MethodInfo methodInfo)
		{
			this._MethodHandle = methodInfo.MethodHandle;
			this.Attributes = methodInfo.ReturnParameter.GetCustomAttributes<Attribute>(true)?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
			this.Type = methodInfo.ReturnType.GetTypeMember();
		}

		private readonly RuntimeMethodHandle _MethodHandle;

		public IImmutableList<Attribute> Attributes { get; }

		public bool IsTask => this.Type.SystemType == SystemType.Task;

		public bool IsValueTask => this.Type.SystemType == SystemType.ValueTask;

		public bool IsVoid => this.Type.SystemType == SystemType.Void;

		public TypeMember Type { get; }

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public bool Equals(ReturnParameter other)
			=> this._MethodHandle == other._MethodHandle;

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public override int GetHashCode()
			=> this._MethodHandle.GetHashCode();
	}
}
