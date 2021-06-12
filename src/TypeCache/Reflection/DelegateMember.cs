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
	public sealed class DelegateMember
		: Member, IEquatable<DelegateMember>
	{
		private const BindingFlags INSTANCE_BINDINGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		private const string METHOD_NAME = "Invoke";

		static DelegateMember()
		{
			Cache = new LazyDictionary<RuntimeTypeHandle, DelegateMember>(handle => new DelegateMember(handle.ToType()));
		}

		internal static IReadOnlyDictionary<RuntimeTypeHandle, DelegateMember> Cache { get; }

		internal DelegateMember(Type type)
			: base(type)
		{
			typeof(Delegate).IsAssignableFrom(type.BaseType).Assert($"{nameof(type)}.{nameof(Type.BaseType)}", true);

			this.Handle = type.TypeHandle;

			var methodInfo = type.GetMethod(METHOD_NAME, INSTANCE_BINDINGS)!;
			this._Invoke = methodInfo.ToInvokeType();
			this.Method = methodInfo.ToDelegate();
			this.MethodHandle = methodInfo.MethodHandle;
			this.Parameters = methodInfo.GetParameters().To(parameter => new MethodParameter(methodInfo.MethodHandle, parameter)).ToImmutableArray();
			this.Return = new ReturnParameter(methodInfo);
		}

		private readonly InvokeType _Invoke;

		public RuntimeTypeHandle Handle { get; }

		public Delegate Method { get; }

		public RuntimeMethodHandle MethodHandle { get; }

		public IImmutableList<MethodParameter> Parameters { get; }

		public ReturnParameter Return { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public object? Invoke(object instance, params object?[]? arguments)
			=> this._Invoke(instance, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(DelegateMember? other)
			=> this.Handle == other?.Handle;
	}
}
