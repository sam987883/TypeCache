// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection
{
	public readonly struct EventMember
		: IMember, IEquatable<EventMember>
	{
		public EventMember(EventInfo eventInfo)
		{
			this.Type = eventInfo.GetTypeMember();
			this.Attributes = eventInfo.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
			this.Name = this.Attributes.First<NameAttribute>()?.Name ?? eventInfo.Name;
			this.Internal = eventInfo.AddMethod!.IsAssembly;
			this.Public = eventInfo.AddMethod!.IsPublic;
			this.EventHandlerType = eventInfo.EventHandlerType!.TypeHandle.GetTypeMember();
			this.AddEventHandler = eventInfo.AddMethod!.MethodHandle.GetMethodMember(this.Type.Handle);
			this.RaiseEvent = eventInfo.RaiseMethod?.MethodHandle.GetMethodMember(this.Type.Handle);
			this.RemoveEventHandler = eventInfo.RemoveMethod!.MethodHandle.GetMethodMember(this.Type.Handle);
		}

		public TypeMember Type { get; }

		public IImmutableList<Attribute> Attributes { get; }

		public string Name { get; }

		public TypeMember EventHandlerType { get; }

		public MethodMember AddEventHandler { get; }

		public MethodMember? RaiseEvent { get; }

		public MethodMember RemoveEventHandler { get; }

		public bool Internal { get; }

		public bool Public { get; }

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
		public bool Equals(EventMember other)
			=> this.AddEventHandler.Handle == other.AddEventHandler.Handle;

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public override int GetHashCode()
			=> this.AddEventHandler.Handle.GetHashCode();
	}
}
