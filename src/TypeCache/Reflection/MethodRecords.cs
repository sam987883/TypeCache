// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection
{
	public sealed record ConstructorMember(string Name, TypeMember Type, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		RuntimeMethodHandle Handle, CreateType? Create, Delegate? Method, IImmutableList<MethodParameter> Parameters, ReturnParameter Return)
		: MethodMember(Name, Type, Attributes, IsInternal, IsPublic, Handle, Method, Parameters, Return)
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ConstructorInfo(ConstructorMember constructorMember)
			=> (ConstructorInfo)ConstructorInfo.GetMethodFromHandle(constructorMember.Handle)!;
	}

	public abstract record MethodMember(string Name, TypeMember Type, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		RuntimeMethodHandle Handle, Delegate? Method, IImmutableList<MethodParameter> Parameters, ReturnParameter Return)
		: Member(Name, Attributes, IsInternal, IsPublic), IEquatable<MethodMember>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual bool Equals(MethodMember? other)
			=> other?.Handle.Equals(this.Handle) is true;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this.Handle.GetHashCode();
	}

	public sealed record InstanceMethodMember(string Name, TypeMember Type, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		RuntimeMethodHandle Handle, Delegate? Method, InvokeType? Invoke, IImmutableList<MethodParameter> Parameters, ReturnParameter Return)
		: MethodMember(Name, Type, Attributes, IsInternal, IsPublic, Handle, Method, Parameters, Return)
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator MethodInfo(InstanceMethodMember methodMember)
			=> (MethodInfo)MethodInfo.GetMethodFromHandle(methodMember.Handle)!;
	}

	public sealed record StaticMethodMember(string Name, TypeMember Type, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		RuntimeMethodHandle Handle, Delegate? Method, StaticInvokeType? Invoke, IImmutableList<MethodParameter> Parameters, ReturnParameter Return)
		: MethodMember(Name, Type, Attributes, IsInternal, IsPublic, Handle, Method, Parameters, Return)
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator MethodInfo(StaticMethodMember methodMember)
			=> (MethodInfo)MethodInfo.GetMethodFromHandle(methodMember.Handle)!;
	}

	public sealed record MethodParameter(string Name, IImmutableList<Attribute> Attributes, bool IsOptional, bool IsOut,
		object? DefaultValue, bool HasDefaultValue, TypeMember Type)
	{
		public bool Equals(MethodParameter? other)
			=> other is not null
				&& this.Name.Is(other.Name)
				&& this.DefaultValue == other.DefaultValue
				&& this.HasDefaultValue == other.HasDefaultValue
				&& this.IsOptional == this.IsOptional
				&& this.IsOut == other.IsOut
				&& this.Type == other.Type;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> HashCode.Combine(this.Name, this.Type, this.DefaultValue, this.HasDefaultValue, this.IsOptional, this.IsOut);
	}

	public sealed record ReturnParameter(IImmutableList<Attribute> Attributes, TypeMember Type)
	{
		public bool IsTask => this.Type.SystemType == SystemType.Task;

		public bool IsValueTask => this.Type.SystemType == SystemType.ValueTask;

		public bool IsVoid => this.Type.SystemType == SystemType.Void;
	}
}
