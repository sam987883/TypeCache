// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace TypeCache.Reflection
{
	public abstract record FieldMember(string Name, TypeMember Type, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		RuntimeFieldHandle Handle, Delegate? Getter, Delegate? Setter, TypeMember FieldType)
		: Member(Name, Attributes, IsInternal, IsPublic), IEquatable<FieldMember>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual bool Equals(FieldMember? other)
			=> other?.Handle.Equals(this.Handle) is true;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this.Handle.GetHashCode();
	}

	public sealed record ConstantMember(string Name, TypeMember Type, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		Delegate? Getter, object Value, TypeMember FieldType)
		: FieldMember(Name, Type, Attributes, IsInternal, IsPublic, default, Getter, null, FieldType);

	public sealed record InstanceFieldMember(string Name, TypeMember Type, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		RuntimeFieldHandle Handle, Delegate? Getter, Delegate? Setter, GetValue? GetValue, SetValue? SetValue, TypeMember FieldType)
		: FieldMember(Name, Type, Attributes, IsInternal, IsPublic, Handle, Getter, Setter, FieldType);

	public sealed record StaticFieldMember(string Name, TypeMember Type, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		RuntimeFieldHandle Handle, Delegate? Getter, Delegate? Setter, StaticGetValue? GetValue, StaticSetValue? SetValue, TypeMember FieldType)
		: FieldMember(Name, Type, Attributes, IsInternal, IsPublic, Handle, Getter, Setter, FieldType);
}
