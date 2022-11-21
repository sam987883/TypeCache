// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Linq;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache;

public static class EventOf<T>
	where T : class
{
	private sealed record HandlerReference(WeakReference<T> Instance, EventMember EventMember, Delegate EventHandler);

	private static IDictionary<long, HandlerReference> EventHandlers { get; } = new Dictionary<long, HandlerReference>();

	public static IReadOnlyList<EventMember> Events => TypeOf<T>.Member.Events;

	public static long AddEventHandler(T instance, string eventMemberName, Delegate handler)
	{
		instance.AssertNotNull();
		eventMemberName.AssertNotBlank();
		handler.AssertNotNull();

		var key = DateTime.UtcNow.Ticks;
		var eventMember = Events.First(_ => _.Name.Is(eventMemberName));
		var reference = new HandlerReference(new WeakReference<T>(instance), eventMember, handler);
		eventMember.Add(instance!, handler);
		EventHandlers.Add(key, reference);
		return key;
	}

	public static Delegate? GetEventHandler(long key)
		=> EventHandlers.TryGetValue(key, out var handler) ? handler.EventHandler : null;

	public static T? GetEventInstance(long key)
		=> EventHandlers.TryGetValue(key, out var handler) && handler.Instance.TryGetTarget(out var target) ? target : null;

	public static bool RemoveEventHandler(long key)
	{
		if (EventHandlers.TryGetValue(key, out var handler))
		{
			if (handler.Instance.TryGetTarget(out var target))
				handler.EventMember.Remove(target, handler.EventHandler);

			return EventHandlers.Remove(key);
		}
		return false;
	}
}
