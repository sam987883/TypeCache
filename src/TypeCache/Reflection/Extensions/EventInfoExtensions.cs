// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Collections.Extensions;

namespace TypeCache.Reflection.Extensions
{
	public static class EventInfoExtensions
	{
		public static EventMember ToMember(this EventInfo @this)
		{
			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var type = MemberCache.Types[@this.EventHandlerType!.TypeHandle];
			var addEventMethodInfo = @this.AddMethod!;
			var raiseEventMethodInfo = @this.RaiseMethod!;
			var removeEventMethodInfo = @this.RemoveMethod!;

			return new EventMember(@this.GetName(), attributes, type, raiseEventMethodInfo.IsAssembly, raiseEventMethodInfo.IsPublic,
				addEventMethodInfo.ToMember(), raiseEventMethodInfo.ToMember(), removeEventMethodInfo.ToMember());
		}

		public static StaticEventMember ToStaticMember(this EventInfo @this)
		{
			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var type = MemberCache.Types[@this.EventHandlerType!.TypeHandle];
			var addEventMethodInfo = @this.AddMethod!;
			var raiseEventMethodInfo = @this.RaiseMethod!;
			var removeEventMethodInfo = @this.RemoveMethod!;

			return new StaticEventMember(@this.GetName(), attributes, type, raiseEventMethodInfo.IsAssembly, raiseEventMethodInfo.IsPublic,
				addEventMethodInfo.ToStaticMember(), raiseEventMethodInfo.ToStaticMember(), removeEventMethodInfo.ToStaticMember());
		}
	}
}
