// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace TypeCache.Reflection
{
	public readonly struct TypeMember : IEquatable<TypeMember>
	{
		public IImmutableList<Attribute> Attributes { get; init; }

		public CollectionType CollectionType { get; init; }

		public bool IsNullable { get; init; }

		public bool IsTask { get; init; }

		public bool IsValueTask { get; init; }

		public string Name { get; init; }

		public NativeType NativeType { get; init; }

		public RuntimeTypeHandle Handle { get; init; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(TypeMember other)
			=> this.Handle.Equals(other.Handle);

		public override bool Equals(object? other)
			=> other switch { TypeMember member => member.Equals(this), _ => false };

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> base.GetHashCode();

		public static bool operator ==(TypeMember a, TypeMember b)
			=> a.Equals(b);

		public static bool operator !=(TypeMember a, TypeMember b)
			=> !a.Equals(b);
	}
}
