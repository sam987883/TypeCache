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
	public readonly struct DelegateMember
		: IMember, IEquatable<DelegateMember>
	{
		private const BindingFlags INSTANCE_BINDINGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		private const string METHOD_NAME = "Invoke";

		static DelegateMember()
		{
			Cache = new LazyDictionary<RuntimeTypeHandle, DelegateMember>(handle => new DelegateMember(handle.ToType()));
		}

		internal static IReadOnlyDictionary<RuntimeTypeHandle, DelegateMember> Cache { get; }

		internal DelegateMember(Type type)
		{
			typeof(Delegate).IsAssignableFrom(type.BaseType).Assert($"{nameof(type)}.{nameof(Type.BaseType)}", true);

			this.Handle = type.TypeHandle;
			this.Attributes = type.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
			this.Name = this.Attributes.First<NameAttribute>()?.Name ?? type.Name;

			var methodInfo = type.GetMethod(METHOD_NAME, INSTANCE_BINDINGS)!;
			this._Invoke = methodInfo.ToInvokeType();
			this.Method = methodInfo.ToDelegate();
			this.MethodHandle = methodInfo.MethodHandle;
			this.Parameters = methodInfo.GetParameters().To(parameter => new MethodParameter(methodInfo.MethodHandle, parameter)).ToImmutableArray();
			this.Return = new ReturnParameter(methodInfo);
			this.Internal = !type.IsVisible;
			this.Public = type.IsPublic;
		}

		private readonly InvokeType _Invoke;

		public IImmutableList<Attribute> Attributes { get; }

		public string Name { get; }

		public RuntimeTypeHandle Handle { get; }

		public Delegate Method { get; }

		public RuntimeMethodHandle MethodHandle { get; }

		public IImmutableList<MethodParameter> Parameters { get; }

		public ReturnParameter Return { get; }

		public bool Internal { get; }

		public bool Public { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public object? Invoke(object instance, params object?[]? arguments)
			=> this._Invoke(instance, arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(DelegateMember other)
			=> this.Handle.Equals(other.Handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this.Handle.GetHashCode();
	}
}
