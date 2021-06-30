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
			: base(eventInfo)
		{
			this.EventHandlerType = eventInfo.EventHandlerType!.TypeHandle.GetTypeMember();
			this.AddEventHandler = eventInfo.AddMethod!.MethodHandle.GetMethodMember(this.Type.Handle);
			this.RaiseEvent = eventInfo.RaiseMethod?.MethodHandle.GetMethodMember(this.Type.Handle);
			this.RemoveEventHandler = eventInfo.RemoveMethod!.MethodHandle.GetMethodMember(this.Type.Handle);
		}

		public TypeMember EventHandlerType { get; }

		public MethodMember AddEventHandler { get; }

		public MethodMember? RaiseEvent { get; }

		public MethodMember RemoveEventHandler { get; }

		public new TypeMember Type => base.Type!;

		/// <param name="instance">Pass null if the event is static.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(object? instance, Delegate handler)
			=> this.AddEventHandler.Invoke(instance, handler);

		/// <param name="instance">Pass null if the event is static.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Raise(object? instance)
			=> this.RaiseEvent?.Invoke(instance);

		/// <param name="instance">Pass null if the event is static.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(object? instance, Delegate handler)
			=> this.RemoveEventHandler.Invoke(instance, handler);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(EventMember? other)
			=> this.AddEventHandler.Handle == other?.AddEventHandler.Handle;
	}
}
