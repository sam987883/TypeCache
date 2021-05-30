// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection
{
	public abstract record Member(string Name, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic);

	public sealed record TypeMember(string Name, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		Kind Kind, SystemType SystemType, RuntimeTypeHandle Handle, RuntimeTypeHandle BaseTypeHandle, RuntimeTypeHandle? EnclosedTypeHandle,
		IImmutableList<RuntimeTypeHandle> GenericTypeHandles, IImmutableList<RuntimeTypeHandle> InterfaceTypeHandles, bool IsEnumerable, bool IsPointer, bool IsRef)
		: Member(Name, Attributes, IsInternal, IsPublic), IEquatable<TypeMember>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Type(TypeMember typeMember)
			=> typeMember.Handle.ToType();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(TypeMember? other)
			=> other?.Equals(this.Handle) is true;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this.Handle.GetHashCode();

		public object Create(params object?[]? parameters)
		{
			var constructor = MemberCache.Constructors[this.Handle].First(constructor => constructor!.IsCallableWith(parameters));
			if (constructor != null)
				return constructor.Create!(parameters);
			throw new ArgumentException($"{this.Name}.{nameof(Create)}(...): no constructor found that takes the {parameters?.Length ?? 0} provided {nameof(parameters)}.");
		}

		public D? GetMethod<D>(string name)
			where D : Delegate
			=> MemberCache.Methods[this.Handle].Get(name).To(_ => _.Method).First<D>();

		public D? GetStaticMethod<D>(string name)
			where D : Delegate
			=> MemberCache.StaticMethods[this.Handle].Get(name).To(_ => _.Method).First<D>();
	}
}
