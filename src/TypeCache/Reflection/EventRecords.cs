// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace TypeCache.Reflection
{
	public sealed record EventMember(string Name, TypeMember Type, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		InstanceMethodMember AddEventHandler, InstanceMethodMember RaiseEvent, InstanceMethodMember RemoveEventHandler, TypeMember EventHandlerType)
		: Member(Name, Attributes, IsInternal, IsPublic), IEquatable<EventMember>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(object instance, Delegate handler)
			=> this.Add(instance, handler);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Raise(object instance)
			=> this.RaiseEvent.Invoke!(instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(object instance, Delegate handler)
			=> this.RemoveEventHandler.Invoke!(instance, handler);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(EventMember? other)
			=> this.RaiseEvent.Handle == other?.RaiseEvent.Handle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this.RaiseEvent.Handle.GetHashCode();
	}

	public sealed record StaticEventMember(string Name, TypeMember Type, IImmutableList<Attribute> Attributes, bool IsInternal, bool IsPublic,
		StaticMethodMember AddEventHandler, StaticMethodMember RaiseEvent, StaticMethodMember RemoveEventHandler, TypeMember EventHandlerType)
		: Member(Name, Attributes, IsInternal, IsPublic), IEquatable<EventMember>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(Delegate handler)
			=> this.AddEventHandler.Invoke!(handler);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Raise()
			=> this.RaiseEvent.Invoke!();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(Delegate handler)
			=> this.RemoveEventHandler.Invoke!(handler);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(EventMember? other)
			=> this.RaiseEvent.Handle == other?.RaiseEvent.Handle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this.RaiseEvent.Handle.GetHashCode();
	}
}
