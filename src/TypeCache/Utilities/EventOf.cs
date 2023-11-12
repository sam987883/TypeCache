// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using TypeCache.Extensions;
using static System.Reflection.BindingFlags;

namespace TypeCache.Utilities;

public static class EventOf<T>
	where T : class
{
	private sealed record HandlerReference(WeakReference<T> Instance, EventInfo EventInfo, Delegate EventHandler);

	private static IDictionary<long, HandlerReference> EventHandlers { get; } = new Dictionary<long, HandlerReference>();

	public static EventInfo[] Events => typeof(T).GetEvents(FlattenHierarchy | Instance | Public);

	public static long AddEventHandler(T instance, string eventMemberName, Delegate handler)
	{
		instance.AssertNotNull();
		eventMemberName.AssertNotBlank();
		handler.AssertNotNull();

		var key = DateTime.UtcNow.Ticks;
		var eventInfo = Events.First(_ => _.Name().Is(eventMemberName));
		var reference = new HandlerReference(instance.ToWeakReference(), eventInfo, handler);
		eventInfo.AddMethod?.InvokeMethod(instance!, handler);
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
				handler.EventInfo.RemoveMethod?.InvokeMethod(target, handler.EventHandler);

			return EventHandlers.Remove(key);
		}

		return false;
	}
}
