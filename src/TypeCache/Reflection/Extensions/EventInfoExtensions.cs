// Copyright (c) 2021 Samuel Abraham

using System;
using System.Reflection;
using TypeCache.Collections.Extensions;

namespace TypeCache.Reflection.Extensions
{
	public static class EventInfoExtensions
	{
		public static EventMember CreateMember(this EventInfo @this)
		{
			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var type = MemberCache.Types[@this.EventHandlerType!.TypeHandle];
			var addEventMethodInfo = @this.AddMethod!;
			var raiseEventMethodInfo = @this.RaiseMethod!;
			var removeEventMethodInfo = @this.RemoveMethod!;

			return new EventMember(@this.GetName(), attributes, type, raiseEventMethodInfo.IsAssembly, raiseEventMethodInfo.IsPublic,
				addEventMethodInfo.CreateMember(), raiseEventMethodInfo.CreateMember(), removeEventMethodInfo.CreateMember());
		}

		public static StaticEventMember CreateStaticMember(this EventInfo @this)
		{
			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var type = MemberCache.Types[@this.EventHandlerType!.TypeHandle];
			var addEventMethodInfo = @this.AddMethod!;
			var raiseEventMethodInfo = @this.RaiseMethod!;
			var removeEventMethodInfo = @this.RemoveMethod!;

			return new StaticEventMember(@this.GetName(), attributes, type, raiseEventMethodInfo.IsAssembly, raiseEventMethodInfo.IsPublic,
				addEventMethodInfo.CreateStaticMember(), raiseEventMethodInfo.CreateStaticMember(), removeEventMethodInfo.CreateStaticMember());
		}
	}
}
