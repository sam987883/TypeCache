// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection
{
	public sealed class MethodMember
		: Member, IEquatable<MethodMember>
	{
		static MethodMember()
		{
			Cache = new LazyDictionary<(RuntimeMethodHandle, RuntimeTypeHandle), MethodMember>(CreateMethodMember);

			static MethodMember CreateMethodMember((RuntimeMethodHandle MethodHandle, RuntimeTypeHandle TypeHandle) handle)
				=> new MethodMember((MethodInfo)handle.TypeHandle.ToMethodBase(handle.MethodHandle)!);
		}

		internal static IReadOnlyDictionary<(RuntimeMethodHandle, RuntimeTypeHandle), MethodMember> Cache { get; }

		internal MethodMember(MethodInfo methodInfo)
			: base(methodInfo, methodInfo.IsAssembly, methodInfo.IsPublic)
		{
			this.Handle = methodInfo.MethodHandle;
			this.Method = methodInfo.ToDelegate();
			this.Parameters = methodInfo.GetParameters().To(parameter => new MethodParameter(methodInfo.MethodHandle, parameter)).ToImmutableArray();
			this.Static = methodInfo.IsStatic;
			this.Type = methodInfo.GetTypeMember();
			this.Return = new ReturnParameter(methodInfo);

			this._Invoke = methodInfo.ToInvokeType();
		}

		private readonly InvokeType _Invoke;

		public RuntimeMethodHandle Handle { get; }

		public Delegate Method { get; }

		public IImmutableList<MethodParameter> Parameters { get; }

		public ReturnParameter Return { get; }

		public bool Static { get; }

		public TypeMember Type { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator MethodInfo(MethodMember member)
			=> (MethodInfo)member.Handle.ToMethodBase()!;

		/// <param name="instance">Pass null if the method is static.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public object? Invoke(object? instance, params object?[]? arguments)
			=> this._Invoke(instance, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(MethodMember? other)
			=> this.Handle == other?.Handle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this.Handle.GetHashCode();
	}
}
