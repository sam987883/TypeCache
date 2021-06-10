// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection
{
	public sealed class ReturnParameter : IEquatable<ReturnParameter>
	{
		internal ReturnParameter(MethodInfo methodInfo)
		{
			this._MethodHandle = methodInfo.MethodHandle;
			this.Attributes = methodInfo.ReturnParameter.GetCustomAttributes<Attribute>().ToImmutableArray();
			this.Type = methodInfo.ReturnType.GetTypeMember();
		}

		private readonly RuntimeMethodHandle _MethodHandle;

		public IImmutableList<Attribute> Attributes { get; }

		public bool IsTask => this.Type.SystemType == SystemType.Task;

		public bool IsValueTask => this.Type.SystemType == SystemType.ValueTask;

		public bool IsVoid => this.Type.SystemType == SystemType.Void;

		public TypeMember Type { get; }

		public bool Equals(ReturnParameter? other)
			=> this._MethodHandle == other?._MethodHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this._MethodHandle.GetHashCode();
	}
}
