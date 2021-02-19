// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

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

		public Func<object?>? GetValue { get; init; }

		public Delegate? Setter { get; init; }

		public Action<object?>? SetValue { get; init; }

		public TypeMember Type { get; init; }

		public bool Equals(StaticFieldMember other)
			=> this.Type.Equals(other.Type) && this.Name.Is(other.Name, true);

		public override bool Equals(object? other)
			=> other switch { StaticFieldMember member => member.Equals(this), _ => false };

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> base.GetHashCode();

		public static bool operator ==(StaticFieldMember a, StaticFieldMember b)
			=> a.Equals(b);

		public static bool operator !=(StaticFieldMember a, StaticFieldMember b)
			=> !a.Equals(b);
	}
}
