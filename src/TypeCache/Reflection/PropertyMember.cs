﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace TypeCache.Reflection
{
	public readonly struct PropertyMember : IEquatable<PropertyMember>
	{
		public IImmutableList<Attribute> Attributes { get; init; }

		public bool IsInternal { get; init; }

		public bool IsPublic { get; init; }

		public string Name { get; init; }

		public MethodMember? Getter { get; init; }

		public GetValue? GetValue { get; init; }

		public MethodMember? Setter { get; init; }

		public SetValue? SetValue { get; init; }

		public TypeMember Type { get; init; }

		public bool Equals(PropertyMember other)
			=> this.Getter?.Handle == other.Getter?.Handle && this.Setter?.Handle == other.Setter?.Handle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? other)
			=> other is PropertyMember member && this.Equals(member);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> base.GetHashCode();

		public static bool operator ==(PropertyMember a, PropertyMember b)
			=> a.Equals(b);

		public static bool operator !=(PropertyMember a, PropertyMember b)
			=> !a.Equals(b);
	}
}