// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection
{
	public readonly struct StaticMethodMember : IEquatable<StaticMethodMember>
	{
		public IImmutableList<Attribute> Attributes { get; init; }

		public RuntimeMethodHandle Handle { get; init; }

		public StaticInvokeType Invoke { get; init; }

		public bool IsInternal { get; init; }

		public bool IsPublic { get; init; }

		public bool IsVoid { get; init; }

		public Delegate Method { get; init; }

		public string Name { get; init; }

		public IImmutableList<Parameter> Parameters { get; init; }

		public IImmutableList<Attribute> ReturnAttributes { get; init; }

		public TypeMember Type { get; init; }

		public bool Equals(StaticMethodMember other)
			=> this.Type.Equals(other.Type) && this.Name.Is(other.Name, true)
				&& this.Parameters.Match(other.Parameters);

		public override bool Equals(object? other)
			=> other switch { StaticMethodMember member => member.Equals(this), _ => false };

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> base.GetHashCode();

		public static bool operator ==(StaticMethodMember a, StaticMethodMember b)
			=> a.Equals(b);

		public static bool operator !=(StaticMethodMember a, StaticMethodMember b)
			=> !a.Equals(b);
	}
}
