// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace TypeCache.Reflection
{
	public readonly struct MethodMember : IEquatable<MethodMember>
	{
		public IImmutableList<Attribute> Attributes { get; init; }

		public RuntimeMethodHandle Handle { get; init; }

		public InvokeType? Invoke { get; init; }

		public bool IsInternal { get; init; }

		public bool IsPublic { get; init; }

		public Delegate? Method { get; init; }

		public string Name { get; init; }

		public IImmutableList<Parameter> Parameters { get; init; }

		public ReturnParameter Return { get; init; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(MethodMember other)
			=> this.Handle.Equals(other.Handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? other)
			=> other is MethodMember member && this.Equals(member);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> base.GetHashCode();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(MethodMember a, MethodMember b)
			=> a.Equals(b);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(MethodMember a, MethodMember b)
			=> !a.Equals(b);
	}
}
