// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace TypeCache.Reflection
{
	public readonly struct StaticPropertyMember : IEquatable<StaticPropertyMember>
	{
		public IImmutableList<Attribute> Attributes { get; init; }

		public bool IsInternal { get; init; }

		public bool IsPublic { get; init; }

		public string Name { get; init; }

		public StaticMethodMember? Getter { get; init; }

		public StaticGetValue? GetValue { get; init; }

		public StaticMethodMember? Setter { get; init; }

		public StaticSetValue? SetValue { get; init; }

		public TypeMember Type { get; init; }

		public bool Equals(StaticPropertyMember other)
			=> this.Getter?.Handle == other.Getter?.Handle && this.Setter?.Handle == other.Setter?.Handle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? other)
			=> other is StaticPropertyMember member && this.Equals(member);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> base.GetHashCode();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(StaticPropertyMember a, StaticPropertyMember b)
			=> a.Equals(b);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(StaticPropertyMember a, StaticPropertyMember b)
			=> !a.Equals(b);
	}
}
