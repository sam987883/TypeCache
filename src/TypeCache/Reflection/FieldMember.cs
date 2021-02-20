// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection
{
	public readonly struct FieldMember : IEquatable<FieldMember>
	{
		public IImmutableList<Attribute> Attributes { get; init; }

		public RuntimeFieldHandle Handle { get; init; }

		public bool IsInternal { get; init; }

		public bool IsPublic { get; init; }

		public string Name { get; init; }

		public Delegate? Getter { get; init; }

		public Func<object, object?>? GetValue { get; init; }

		public Delegate? Setter { get; init; }

		public Action<object, object?>? SetValue { get; init; }

		public TypeMember Type { get; init; }

		public bool Equals(FieldMember other)
			=> this.Type.Equals(other.Type) && this.Name.Is(other.Name, true);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? other)
			=> (other is FieldMember member) ? this.Equals(member) : false;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> base.GetHashCode();

		public static bool operator ==(FieldMember a, FieldMember b)
			=> a.Equals(b);

		public static bool operator !=(FieldMember a, FieldMember b)
			=> !a.Equals(b);
	}
}
