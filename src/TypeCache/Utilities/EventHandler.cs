// Copyright (c) 2021 Samuel Abraham

using System.Collections.Concurrent;
using System.Reflection;
using TypeCache.Extensions;
using static System.Reflection.BindingFlags;

namespace TypeCache.Utilities;

public static class EventHandler<T>
	where T : class
{
	private readonly record struct EventHandlerReference(WeakReference<T> Instance, RuntimeMethodHandle? AddMethodHandle, RuntimeMethodHandle? RemoveMethodHandle, Delegate EventHandler);

	private static readonly IDictionary<long, EventHandlerReference> EventHandlers = new ConcurrentDictionary<long, EventHandlerReference>();

	public static EventInfo[] Events => typeof(T).GetEvents(FlattenHierarchy | Instance | Public);

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentException"/>
	public static long Add(T instance, string eventMemberName, Delegate handler)
	{
		instance.ThrowIfNull();
		eventMemberName.ThrowIfBlank();
		handler.ThrowIfNull();

		var eventInfo = typeof(T).GetEvent(eventMemberName);
		eventInfo.ThrowIfNull();
		eventInfo.AddMethod.ThrowIfNull();

		var key = DateTime.UtcNow.Ticks;
		var reference = new EventHandlerReference(instance.ToWeakReference(), eventInfo.AddMethod.MethodHandle, eventInfo.RemoveMethod?.MethodHandle, handler);
		EventHandlers.Add(key, reference);
		eventInfo.AddMethod.ToMethodEntity()!.Invoke(instance, [handler]);
		return key;
	}

	public static Delegate? GetEventHandler(long key)
		=> EventHandlers.TryGetValue(key, out var handler) ? handler.EventHandler : null;

	public static T? GetEventInstance(long key)
		=> EventHandlers.TryGetValue(key, out var handler) && handler.Instance.TryGetTarget(out var target) ? target : null;

	public static bool Remove(long key)
	{
		if (!EventHandlers.TryGetValue(key, out var handler))
			return false;

		if (handler.Instance.TryGetTarget(out var target))
			((MethodInfo)handler.RemoveMethodHandle?.ToMethodBase()!).ToMethodEntity()!.Invoke(target, [handler.EventHandler]);

		return EventHandlers.Remove(key);
	}
}
