// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace TypeCache.Reflection
{
	public readonly struct StaticFieldMember : IEquatable<StaticFieldMember>
	{
		public IImmutableList<Attribute> Attributes { get; init; }

		public RuntimeFieldHandle Handle { get; init; }

		public bool Internal { get; init; }

		public string Name { get; init; }

		public bool Public { get; init; }

		public Delegate? Getter { get; init; }

		public StaticGetValue? GetValue { get; init; }

		public Delegate? Setter { get; init; }

		public StaticSetValue? SetValue { get; init; }

		public TypeMember Type { get; init; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(StaticFieldMember other)
			=> this.Handle.Equals(other.Handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? other)
			=> other is StaticFieldMember member && this.Equals(member);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> base.GetHashCode();

		public static bool operator ==(StaticFieldMember a, StaticFieldMember b)
			=> a.Equals(b);

		public static bool operator !=(StaticFieldMember a, StaticFieldMember b)
			=> !a.Equals(b);
	}
}
