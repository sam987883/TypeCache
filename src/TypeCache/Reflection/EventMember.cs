// Copyright (c) 2021 Samuel Abraham

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection
{
	public sealed class EventMember
		: Member, IEquatable<EventMember>
	{
		public EventMember(EventInfo eventInfo)
			: base(eventInfo, eventInfo.RaiseMethod!.IsAssembly, eventInfo.RaiseMethod.IsPublic)
		{
			this.Type = eventInfo.DeclaringType!.GetTypeMember();
			this.AddEventHandler = eventInfo.AddMethod!.MethodHandle.GetMethodMember();
			this.RaiseEvent = eventInfo.RaiseMethod.MethodHandle.GetMethodMember();
			this.RemoveEventHandler = eventInfo.RemoveMethod!.MethodHandle.GetMethodMember();
			this.EventHandlerType = eventInfo.EventHandlerType!.TypeHandle.GetTypeMember();
		}

		public TypeMember Type { get; }

		public MethodMember AddEventHandler { get; }

		public MethodMember RaiseEvent { get; }

		public MethodMember RemoveEventHandler { get; }

		public TypeMember EventHandlerType { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(object instance, Delegate handler)
			=> this.AddEventHandler.Invoke(instance, handler);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Raise(object instance)
			=> this.RaiseEvent.Invoke(instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(object instance, Delegate handler)
			=> this.RemoveEventHandler.Invoke(instance, handler);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(EventMember? other)
			=> this.RaiseEvent.Handle == other?.RaiseEvent.Handle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this.RaiseEvent.Handle.GetHashCode();
	}
}
