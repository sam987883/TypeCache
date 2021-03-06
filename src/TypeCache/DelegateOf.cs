﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TypeCache.Reflection;

namespace TypeCache
{
	public static class DelegateOf<T>
		where T : Delegate
	{
		private static readonly DelegateMember Member = DelegateMember.Cache[typeof(T).TypeHandle];

		public static IImmutableList<Attribute> Attributes => Member.Attributes;

		public static RuntimeTypeHandle Handle => Member.Handle;

		public static bool Internal => Member.Internal;

		public static Delegate Method => Member.Method!;

		public static RuntimeMethodHandle MethodHandle => Member.MethodHandle;

		public static string Name => Member.Name;

		public static IImmutableList<MethodParameter> Parameters => Member.Parameters;

		public static bool Public => Member.Public;

		public static ReturnParameter Return => Member.Return;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object? Invoke(object instance, params object?[]? arguments)
			=> Member.Invoke(instance, arguments);
	}
}
