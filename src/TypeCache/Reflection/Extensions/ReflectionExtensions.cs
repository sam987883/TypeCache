// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Extensions
{
	public static class ReflectionExtensions
	{
		public static string GetName(this MemberInfo @this)
			=> @this.GetCustomAttribute<NameAttribute>()?.Name ?? (@this.Name.Contains('`') ? @this.Name.Left(@this.Name.IndexOf('`')) : @this.Name);

		public static bool IsInvokable(this ConstructorInfo @this)
			=> @this.GetParameters().All(_ => !_.IsOut && _.ParameterType.IsInvokable());

		public static bool IsInvokable(this MethodInfo @this)
			=> @this.GetParameters().All(_ => !_.IsOut && _.ParameterType.IsInvokable()) && !@this.ReturnType.IsByRef && !@this.ReturnType.IsByRefLike;
	}
}
