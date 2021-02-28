// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace TypeCache.Reflection
{
	public readonly struct ConstructorMember
	{
		public IImmutableList<Attribute> Attributes { get; init; }

		public CreateType? Create { get; init; }

		public RuntimeMethodHandle Handle { get; init; }

		public bool IsInternal { get; init; }

		public bool IsPublic { get; init; }

		public Delegate? Method { get; init; }

		public IImmutableList<Parameter> Parameters { get; init; }

		public TypeMember Type { get; init; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(ConstructorMember other)
			=> this.Handle.Equals(other.Handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? other)
			=> other is ConstructorMember member && this.Equals(member);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> base.GetHashCode();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(ConstructorMember a, ConstructorMember b)
			=> a.Equals(b);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(ConstructorMember a, ConstructorMember b)
			=> !a.Equals(b);
	}
}
