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

		public IImmutableList<ConstructorMember> Constructors => MemberCache.Constructors[this.Handle];

		public IImmutableDictionary<string, EventMember> Events => MemberCache.Events[this.Handle];

		public IImmutableDictionary<string, StaticEventMember> StaticEvents => MemberCache.StaticEvents[this.Handle];

		public IImmutableDictionary<string, InstanceFieldMember> Fields => MemberCache.Fields[this.Handle];

		public IImmutableList<IndexerMember> Indexers => MemberCache.Indexers[this.Handle];

		public IImmutableDictionary<string, IImmutableList<InstanceMethodMember>> Methods => MemberCache.Methods[this.Handle];

		public IImmutableDictionary<string, InstancePropertyMember> Properties => MemberCache.Properties[this.Handle];

		public IImmutableDictionary<string, StaticFieldMember> StaticFields => MemberCache.StaticFields[this.Handle];

		public IImmutableDictionary<string, IImmutableList<StaticMethodMember>> StaticMethods => MemberCache.StaticMethods[this.Handle];

		public IImmutableDictionary<string, StaticPropertyMember> StaticProperties => MemberCache.StaticProperties[this.Handle];

		public object Create(params object?[]? parameters)
		{
			var constructor = this.Constructors.First(constructor => constructor!.IsCallableWith(parameters));
			if (constructor != null)
				return constructor.Create!(parameters);
			throw new ArgumentException($"{this.Name}.{nameof(Create)}(...): no constructor found that takes the {parameters?.Length ?? 0} provided {nameof(parameters)}.");
		}

		public D? GetMethod<D>(string name)
			where D : Delegate
			=> this.Methods.Get(name).To(_ => _.Method).First<D>();

		public D? GetStaticMethod<D>(string name)
			where D : Delegate
			=> this.StaticMethods.Get(name).To(_ => _.Method).First<D>();
	}
}
