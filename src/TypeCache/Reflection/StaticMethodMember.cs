// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace TypeCache.Reflection
{
	public readonly struct StaticMethodMember : IEquatable<StaticMethodMember>
	{
		public IImmutableList<Attribute> Attributes { get; init; }

		public RuntimeMethodHandle Handle { get; init; }

		public StaticInvokeType Invoke { get; init; }

		public bool IsInternal { get; init; }

		public bool IsPublic { get; init; }

		public Delegate Method { get; init; }

		public string Name { get; init; }

		public IImmutableList<Parameter> Parameters { get; init; }

		public ReturnParameter Return { get; init; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(StaticMethodMember other)
			=> this.Handle.Equals(other.Handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? other)
			=> other is StaticMethodMember member && this.Equals(member);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> base.GetHashCode();

		public static bool operator ==(StaticMethodMember a, StaticMethodMember b)
			=> a.Equals(b);

		public static bool operator !=(StaticMethodMember a, StaticMethodMember b)
			=> !a.Equals(b);
	}
}
