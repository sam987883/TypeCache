// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Reflection
{
	public readonly struct StaticPropertyMember : IEquatable<StaticPropertyMember>
	{
		public IImmutableList<Attribute> Attributes { get; init; }

		public bool IsInternal { get; init; }

		public bool IsPublic { get; init; }

		public string Name { get; init; }

		public TypeMember Type { get; init; }

		public StaticMethodMember? Getter { get; init; }

		public StaticMethodMember? Setter { get; init; }

		public bool Equals(StaticPropertyMember other)
			=> this.Type.Equals(other.Type) && this.Name.Is(other.Name, true);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? other)
			=> (other is StaticPropertyMember member) ? this.Equals(member) : false;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> base.GetHashCode();

		public static bool operator ==(StaticPropertyMember a, StaticPropertyMember b)
			=> a.Equals(b);

		public static bool operator !=(StaticPropertyMember a, StaticPropertyMember b)
			=> !a.Equals(b);
	}
}
