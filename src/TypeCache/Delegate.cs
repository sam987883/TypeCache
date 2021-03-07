// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;

namespace TypeCache
{
	public static class Delegate<T>
		where T : Delegate
	{
		private static readonly MethodMember MethodMember = MemberCache.Delegates[typeof(T).TypeHandle];

		public static RuntimeMethodHandle Handle => MethodMember.Handle;

		public static InvokeType Invoke => MethodMember.Invoke!;

		public static bool IsInternal { get; } = !typeof(T).IsVisible;

		public static bool IsPublic { get; } = typeof(T).IsPublic;

		public static Delegate Method => MethodMember.Method!;

		public static string Name { get; } = typeof(T).GetName();

		public static IImmutableList<Parameter> Parameters => MethodMember.Parameters;

		public static ReturnParameter ReturnParameter => MethodMember.Return;
	}
}
