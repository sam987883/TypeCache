// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;

namespace TypeCache
{
	public static class TypeOf<T>
	{
		private static readonly TypeMember TypeMember = MemberCache.Types[typeof(T).TypeHandle];

		public static IImmutableList<Attribute> Attributes => TypeMember.Attributes;

		public static RuntimeTypeHandle? BaseTypeHandle => TypeMember.BaseTypeHandle;

		public static TypeMember? BaseType { get; } = TypeMember.BaseTypeHandle.HasValue ? MemberCache.Types[TypeMember.BaseTypeHandle.Value] : null;

		public static TypeMember? EnclosedType { get; } = TypeMember.EnclosedTypeHandle.HasValue ? MemberCache.Types[TypeMember.EnclosedTypeHandle.Value] : null;

		public static IImmutableList<TypeMember> GenericTypes { get; } = TypeMember.GenericTypeHandles.To(handle => MemberCache.Types[handle]).ToImmutableArray();

		public static RuntimeTypeHandle Handle => TypeMember.Handle;

		public static IImmutableList<TypeMember> InterfaceTypes { get; } = TypeMember.InterfaceTypeHandles.To(handle => MemberCache.Types[handle]).ToImmutableArray();

		public static bool Is<T2>() => TypeMember.Handle.Is<T2>();

		public static bool Is(Type type) => TypeMember.Handle.Is(type);

		public static bool IsEnumerable => TypeMember.IsEnumerable;

		public static bool IsInternal => TypeMember.IsInternal;

		public static bool IsPublic => TypeMember.IsPublic;

		public static Kind Kind => TypeMember.Kind;

		public static string Name => TypeMember.Name;

		public static SystemType SystemType => TypeMember.SystemType;

		public static IImmutableList<ConstructorMember> Constructors => MemberCache.Constructors[Handle];

		public static IImmutableDictionary<string, EventMember> Events => MemberCache.Events[Handle];

		public static IImmutableDictionary<string, FieldMember> Fields => MemberCache.Fields[Handle];

		public static IImmutableList<IndexerMember> Indexers => MemberCache.Indexers[Handle];

		public static IImmutableDictionary<string, IImmutableList<MethodMember>> Methods => MemberCache.Methods[Handle];

		public static IImmutableDictionary<string, PropertyMember> Properties => MemberCache.Properties[Handle];

		public static IImmutableDictionary<string, StaticEventMember> StaticEvents => MemberCache.StaticEvents[Handle];

		public static IImmutableDictionary<string, StaticFieldMember> StaticFields => MemberCache.StaticFields[Handle];

		public static IImmutableDictionary<string, IImmutableList<StaticMethodMember>> StaticMethods => MemberCache.StaticMethods[Handle];

		public static IImmutableDictionary<string, StaticPropertyMember> StaticProperties => MemberCache.StaticProperties[Handle];

		public static long AddEventHandler(object instance, string eventMemberName, Delegate handler)
		{
			var key = DateTime.UtcNow.Ticks;
			var eventMember = Events[eventMemberName];
			var reference = new EventHandlerReference(new WeakReference(instance), eventMember, handler);
			eventMember.Add(instance, handler);
			MemberCache.EventHandlers.Add(key, reference);
			return key;
		}

		public static long AddStaticEventHandler(string eventMemberName, Delegate handler)
		{
			var key = DateTime.UtcNow.Ticks;
			var staticEventMember = StaticEvents[eventMemberName];
			var reference = new StaticEventHandlerReference(staticEventMember, handler);
			staticEventMember.Add(handler);
			MemberCache.StaticEventHandlers.Add(key, reference);
			return key;
		}

		public static T Create(params object[] parameters)
			=> (T?)Constructors.First(constructor => constructor!.IsCallableWith(parameters))?.Create!(parameters)
				?? throw new ArgumentException($"TypeOf<{Name}>.{nameof(Create)}: failed with {parameters?.Length ?? 0} parameters.");

		public static D? GetMethod<D>(string name)
			where D : Delegate
			=> Methods.Get(name).To(_ => _.Method).First<D>();

		public static D? GetStaticMethod<D>(string name)
			where D : Delegate
			=> StaticMethods.Get(name).To(_ => _.Method).First<D>();

		public static bool RemoveEventHandler(long handlerKey)
		{
			if (MemberCache.EventHandlers.TryGetValue(handlerKey, out var value))
			{
				if (value.InstanceReference.IsAlive)
					value.EventMember.Remove(value.InstanceReference.Target!, value.EventHandler);

				return MemberCache.EventHandlers.Remove(handlerKey);
			}
			return false;
		}

		public static bool RemoveStaticEventHandler(long handlerKey)
		{
			if (MemberCache.StaticEventHandlers.TryGetValue(handlerKey, out var value))
			{
				value.StaticEventMember.Remove(value.EventHandler);
				return MemberCache.EventHandlers.Remove(handlerKey);
			}
			return false;
		}
	}
}
