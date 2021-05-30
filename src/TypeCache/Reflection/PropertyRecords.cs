// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace TypeCache.Reflection
{
	public sealed record InstancePropertyMember(string Name, TypeMember Type, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		InstanceMethodMember? Getter, InstanceMethodMember? Setter, GetValue? GetValue, SetValue? SetValue, TypeMember PropertyType)
		: Member(Name, Attributes, IsInternal, IsPublic), IEquatable<InstancePropertyMember>
	{
		public bool Equals(InstancePropertyMember? other)
			=> other != null && (this.Getter ?? this.Setter)!.Handle == (other.Getter ?? other.Setter)!.Handle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> (this.Getter ?? this.Setter)!.Handle.GetHashCode();
	}

	public sealed record IndexerMember(string Name, TypeMember Type, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		InstanceMethodMember? Getter, InstanceMethodMember? Setter, TypeMember PropertyType)
		: Member(Name, Attributes, IsInternal, IsPublic), IEquatable<IndexerMember>
	{
		public bool Equals(IndexerMember? other)
			=> other != null && (this.Getter ?? this.Setter)!.Handle == (other.Getter ?? other.Setter)!.Handle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> (this.Getter ?? this.Setter)!.Handle.GetHashCode();
	}

	public sealed record StaticPropertyMember(string Name, TypeMember Type, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		StaticMethodMember? Getter, StaticMethodMember? Setter, StaticGetValue? GetValue, StaticSetValue? SetValue, TypeMember PropertyType)
		: Member(Name, Attributes, IsInternal, IsPublic), IEquatable<StaticPropertyMember>
	{
		public bool Equals(StaticPropertyMember? other)
			=> other != null && (this.Getter ?? this.Setter)!.Handle == (other.Getter ?? other.Setter)!.Handle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> (this.Getter ?? this.Setter)!.Handle.GetHashCode();
	}
}
