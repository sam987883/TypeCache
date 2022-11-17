// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

public sealed class EventMember : IMember, IEquatable<EventMember>
{
	public EventMember(EventInfo eventInfo, TypeMember type)
	{
		eventInfo.AddMethod.AssertNotNull();
		eventInfo.EventHandlerType.AssertNotNull();
		eventInfo.RemoveMethod.AssertNotNull();

		this.Attributes = eventInfo.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
		this.Internal = eventInfo.AddMethod.IsAssembly;
		this.Name = eventInfo.Name();
		this.Public = eventInfo.AddMethod.IsPublic;
		this.Type = type;

		this.AddEventHandler = new MethodMember(eventInfo.AddMethod, type);
		this.EventHandlerType = eventInfo.EventHandlerType.TypeHandle.GetTypeMember();
		this.RaiseEvent = eventInfo.RaiseMethod is not null ? new MethodMember(eventInfo.RaiseMethod, type) : null;
		this.RemoveEventHandler = new MethodMember(eventInfo.RemoveMethod, type);
	}

	public MethodMember AddEventHandler { get; }

	/// <inheritdoc/>
	public IReadOnlyList<Attribute> Attributes { get; }

	public TypeMember EventHandlerType { get; }

	/// <inheritdoc cref="MethodBase.IsAssembly"/>
	public bool Internal { get; }

	/// <inheritdoc/>
	public string Name { get; }

	/// <inheritdoc cref="MethodBase.IsPublic"/>
	public bool Public { get; }

	public MethodMember? RaiseEvent { get; }

	public MethodMember RemoveEventHandler { get; }

	/// <summary>
	/// The <see cref="TypeMember"/> that owns this event.
	/// </summary>
	public TypeMember Type { get; }

	/// <param name="instance">Pass null if the event is static.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public void Add(object? instance, Delegate handler)
		=> this.AddEventHandler.Invoke(instance, handler);

	/// <param name="instance">Pass null if the event is static.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public void Raise(object? instance)
		=> this.RaiseEvent?.Invoke(instance);

	/// <param name="instance">Pass null if the event is static.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public void Remove(object? instance, Delegate handler)
		=> this.RemoveEventHandler.Invoke(instance, handler);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Equals([NotNullWhen(true)] EventMember? other)
		=> this.AddEventHandler == other?.AddEventHandler;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> this.Equals(item as EventMember);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> this.AddEventHandler.Handle.GetHashCode();
}
