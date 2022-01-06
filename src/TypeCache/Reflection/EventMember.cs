// Copyright (c) 2021 Samuel Abraham

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

public class EventMember : Member, IEquatable<EventMember>
{
	public EventMember(EventInfo eventInfo, TypeMember type) : base(eventInfo)
	{
		this.Type = type;
		this.EventHandlerType = eventInfo.EventHandlerType!.TypeHandle.GetTypeMember();
		this.AddEventHandler = new MethodMember(eventInfo.AddMethod!, type);
		this.RaiseEvent = eventInfo.RaiseMethod is not null ? new MethodMember(eventInfo.RaiseMethod, type) : null;
		this.RemoveEventHandler = new MethodMember(eventInfo.RemoveMethod!, type);
	}

	public TypeMember Type { get; }

	public TypeMember EventHandlerType { get; }

	public MethodMember AddEventHandler { get; }

	public MethodMember? RaiseEvent { get; }

	public MethodMember RemoveEventHandler { get; }

	/// <param name="instance">Pass null if the event is static.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public void Add(object? instance, Delegate handler)
		=> this.AddEventHandler.Invoke(instance, handler);

	/// <param name="instance">Pass null if the event is static.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public void Raise(object? instance)
		=> this.RaiseEvent?.Invoke(instance);

	/// <param name="instance">Pass null if the event is static.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public void Remove(object? instance, Delegate handler)
		=> this.RemoveEventHandler.Invoke(instance, handler);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public bool Equals(EventMember? other)
		=> other?.AddEventHandler is not null && this.AddEventHandler.Handle.Equals(other.AddEventHandler.Handle);
}
