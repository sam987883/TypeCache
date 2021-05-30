// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace TypeCache.Reflection
{
	public abstract record Member(string Name, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic);

	public sealed record TypeMember(string Name, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		Kind Kind, SystemType SystemType, RuntimeTypeHandle Handle, RuntimeTypeHandle BaseTypeHandle, RuntimeTypeHandle? EnclosedTypeHandle,
		IImmutableList<RuntimeTypeHandle> GenericTypeHandles, IImmutableList<RuntimeTypeHandle> InterfaceTypeHandles, bool IsEnumerable, bool IsPointer, bool IsRef)
		: Member(Name, Attributes, IsInternal, IsPublic), IEquatable<TypeMember>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(TypeMember? other)
			=> other?.Equals(this.Handle) is true;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this.Handle.GetHashCode();
	}
}
