// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace TypeCache.Reflection
{
	public abstract record Member(string Name, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic);

	public sealed record ConstructorMember(string Name, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		RuntimeMethodHandle Handle, CreateType? Create, Delegate? Method, IImmutableList<Parameter> Parameters, TypeMember Type)
		: Member(Name, Attributes, IsInternal, IsPublic), IEquatable<ConstructorMember>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(ConstructorMember? other)
			=> other != null && this.Handle.Equals(other.Handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this.Handle.GetHashCode();
	}

	public sealed record FieldMember(string Name, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		RuntimeFieldHandle Handle, Delegate? Getter, GetValue? GetValue, Delegate? Setter, SetValue? SetValue, TypeMember Type)
		: Member(Name, Attributes, IsInternal, IsPublic), IEquatable<FieldMember>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(FieldMember? other)
			=> other != null && this.Handle.Equals(other.Handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this.Handle.GetHashCode();
	}

	public sealed record StaticFieldMember(string Name, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		RuntimeFieldHandle Handle, Delegate? Getter, StaticGetValue? GetValue, Delegate? Setter, StaticSetValue? SetValue, TypeMember Type)
		: Member(Name, Attributes, IsInternal, IsPublic), IEquatable<StaticFieldMember>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(StaticFieldMember? other)
			=> other != null && this.Handle.Equals(other.Handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this.Handle.GetHashCode();
	}

	public sealed record IndexerMember(string Name, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		MethodMember? GetMethod, MethodMember? SetMethod, TypeMember Type)
		: Member(Name, Attributes, IsInternal, IsPublic);

	public sealed record MethodMember(string Name, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		RuntimeMethodHandle Handle, InvokeType? Invoke, Delegate? Method, IImmutableList<Parameter> Parameters, ReturnParameter Return)
		: Member(Name, Attributes, IsInternal, IsPublic), IEquatable<MethodMember>
	{

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(MethodMember? other)
			=> other != null && this.Handle.Equals(other.Handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this.Handle.GetHashCode();
	}

	public sealed record StaticMethodMember(string Name, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		RuntimeMethodHandle Handle, StaticInvokeType? Invoke, Delegate? Method, IImmutableList<Parameter> Parameters, ReturnParameter Return)
		: Member(Name, Attributes, IsInternal, IsPublic), IEquatable<StaticMethodMember>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(StaticMethodMember? other)
			=> other != null && this.Handle.Equals(other.Handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this.Handle.GetHashCode();
	}

	public sealed record Parameter(string Name, IImmutableList<Attribute> Attributes, bool IsOptional, bool IsOut,
		object? DefaultValue, bool HasDefaultValue, TypeMember Type);

	public sealed record ReturnParameter(IImmutableList<Attribute> Attributes, TypeMember Type)
	{
		public bool IsTask => this.Type.SystemType == SystemType.Task;

		public bool IsValueTask => this.Type.SystemType == SystemType.ValueTask;

		public bool IsVoid => this.Type.SystemType == SystemType.Void;
	}

	public sealed record PropertyMember(string Name, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		MethodMember? Getter, GetValue? GetValue, MethodMember? Setter, SetValue? SetValue, TypeMember Type)
		: Member(Name, Attributes, IsInternal, IsPublic), IEquatable<PropertyMember>
	{
		public bool Equals(PropertyMember? other)
			=> this.Getter?.Handle == other?.Getter?.Handle && this.Setter?.Handle == other?.Setter?.Handle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> (this.Getter ?? this.Setter)!.Handle.GetHashCode();
	}

	public sealed record StaticPropertyMember(string Name, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		StaticMethodMember? Getter, StaticGetValue? GetValue, StaticMethodMember? Setter, StaticSetValue? SetValue, TypeMember Type)
		: Member(Name, Attributes, IsInternal, IsPublic), IEquatable<StaticPropertyMember>
	{
		public bool Equals(StaticPropertyMember? other)
			=> this.Getter?.Handle == other?.Getter?.Handle && this.Setter?.Handle == other?.Setter?.Handle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> (this.Getter ?? this.Setter)!.Handle.GetHashCode();
	}

	public sealed record TypeMember(string Name, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		Kind Kind, SystemType SystemType, RuntimeTypeHandle Handle, RuntimeTypeHandle? BaseTypeHandle, IImmutableList<RuntimeTypeHandle> GenericTypeHandles,
		IImmutableList<RuntimeTypeHandle> InterfaceTypeHandles, bool IsEnumerable, bool IsNullable, bool IsPointer, bool IsRef)
		: Member(Name, Attributes, IsInternal, IsPublic), IEquatable<TypeMember>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(TypeMember? other)
			=> other != null && this.Handle.Equals(other.Handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this.Handle.GetHashCode();
	}
}
