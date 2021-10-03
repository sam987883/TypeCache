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
using static TypeCache.Default;

namespace TypeCache.Reflection
{
	public readonly struct MethodMember
		: IMember, IEquatable<MethodMember>
	{
		static MethodMember()
		{
			Cache = new LazyDictionary<(RuntimeMethodHandle, RuntimeTypeHandle), MethodMember>(CreateMethodMember);

			static MethodMember CreateMethodMember((RuntimeMethodHandle MethodHandle, RuntimeTypeHandle TypeHandle) handle)
				=> new MethodMember((MethodInfo)handle.TypeHandle.ToMethodBase(handle.MethodHandle)!);
		}

		private readonly IReadOnlyDictionary<RuntimeTypeHandle[], InvokeType>? _Cache;

		private readonly InvokeType? _Invoke;

		internal MethodMember(MethodInfo methodInfo)
		{
			this.Type = methodInfo.GetTypeMember();
			this.Attributes = methodInfo.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
			this.Name = this.Attributes.First<NameAttribute>()?.Name ?? methodInfo.Name;
			this.GenericTypes = methodInfo.GetGenericArguments().Length;
			this.Handle = methodInfo.MethodHandle;
			this.Method = !methodInfo.ContainsGenericParameters ? LambdaExpressionFactory.Create(methodInfo).Compile() : null;
			this.Parameters = methodInfo.GetParameters().To(parameter => new MethodParameter(methodInfo.MethodHandle, parameter)).ToImmutableArray();
			this.Static = methodInfo.IsStatic;
			this.Return = new ReturnParameter(methodInfo);
			this.Internal = methodInfo.IsAssembly;
			this.Public = methodInfo.IsPublic;

			this._Cache = methodInfo.ContainsGenericParameters ? new LazyDictionary<RuntimeTypeHandle[], InvokeType>(CreateGenericInvoke, Default.RuntimeTypeHandleArrayComparer) : null;
			this._Invoke = !methodInfo.ContainsGenericParameters ? DelegateExpressionFactory.Create(methodInfo).Compile() : null;

			InvokeType CreateGenericInvoke(params RuntimeTypeHandle[] handles)
			{
				var types = handles.To(handle => handle.ToType()).ToArray();
				return DelegateExpressionFactory.Create(methodInfo.MakeGenericMethod(types)).Compile();
			}
		}

		public IImmutableList<Attribute> Attributes { get; }

		public int GenericTypes { get; }

		public RuntimeMethodHandle Handle { get; }

		public bool Internal { get; }

		public Delegate? Method { get; }

		public string Name { get; }

		public IImmutableList<MethodParameter> Parameters { get; }

		public bool Public { get; }

		public ReturnParameter Return { get; }

		public bool Static { get; }

		public TypeMember Type { get; }

		internal static IReadOnlyDictionary<(RuntimeMethodHandle, RuntimeTypeHandle), MethodMember> Cache { get; }

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public bool Equals(MethodMember other)
			=> this.Handle == other.Handle;

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public override int GetHashCode()
			=> this.Handle.GetHashCode();

		/// <param name="instance">Pass null if the method is static.</param>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public object? Invoke(object? instance, params object?[]? arguments)
			=> this._Invoke?.Invoke(instance, arguments);

		/// <param name="instance">Pass null if the method is static.</param>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public object? InvokeGeneric(object? instance, Type[] genericTypes, params object?[]? arguments)
			=> this._Cache?[genericTypes.To(type => type.TypeHandle).ToArray()].Invoke(instance, arguments);

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static implicit operator MethodInfo(MethodMember member)
			=> (MethodInfo)member.Type.Handle.ToMethodBase(member.Handle)!;
	}
}
