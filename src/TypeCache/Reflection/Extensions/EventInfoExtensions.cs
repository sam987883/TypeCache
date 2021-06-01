// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Extensions
{
	public static class EventInfoExtensions
	{
		public static EventMember ToMember(this EventInfo @this)
		{
			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var addEventMethodInfo = @this.AddMethod!;
			var raiseEventMethodInfo = @this.RaiseMethod!;
			var removeEventMethodInfo = @this.RemoveMethod!;
			var eventHandlerType = @this.EventHandlerType!.GetTypeMember();
			var type = @this.DeclaringType!.GetTypeMember();

			return new EventMember(@this.GetName(), type, attributes, raiseEventMethodInfo.IsAssembly, raiseEventMethodInfo.IsPublic,
				addEventMethodInfo.ToMember(), raiseEventMethodInfo.ToMember(), removeEventMethodInfo.ToMember(), eventHandlerType);
		}

		public static StaticEventMember ToStaticMember(this EventInfo @this)
		{
			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var addEventMethodInfo = @this.AddMethod!;
			var raiseEventMethodInfo = @this.RaiseMethod!;
			var removeEventMethodInfo = @this.RemoveMethod!;
			var eventHandlerType = @this.EventHandlerType!.GetTypeMember();
			var type = @this.DeclaringType!.GetTypeMember();

			return new StaticEventMember(@this.GetName(), type, attributes, raiseEventMethodInfo.IsAssembly, raiseEventMethodInfo.IsPublic,
				addEventMethodInfo.ToStaticMember(), raiseEventMethodInfo.ToStaticMember(), removeEventMethodInfo.ToStaticMember(), eventHandlerType);
		}
	}
}
