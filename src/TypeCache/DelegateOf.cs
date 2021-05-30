// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using TypeCache.Reflection;

namespace TypeCache
{
	public static class DelegateOf<T>
		where T : Delegate
	{
		private static readonly InstanceMethodMember MethodMember = MemberCache.Delegates[typeof(T).TypeHandle];

		public static IImmutableList<Attribute> Attributes => MethodMember.Attributes;

		public static RuntimeMethodHandle Handle => MethodMember.Handle;

		public static InvokeType Invoke => MethodMember.Invoke!;

		public static bool IsInternal => MethodMember.IsInternal;

		public static bool IsPublic => MethodMember.IsPublic;

		public static Delegate Method => MethodMember.Method!;

		public static string Name => MethodMember.Name;

		public static IImmutableList<MethodParameter> Parameters => MethodMember.Parameters;

		public static ReturnParameter Return => MethodMember.Return;
	}
}
