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
	public sealed class ConstructorMember
		: Member, IEquatable<MethodMember>
	{
		static ConstructorMember()
		{
			Cache = new LazyDictionary<(RuntimeMethodHandle, RuntimeTypeHandle), ConstructorMember>(CreateConstructorMember);

			static ConstructorMember CreateConstructorMember((RuntimeMethodHandle MethodHandle, RuntimeTypeHandle TypeHandle) handle)
				=> new ConstructorMember((ConstructorInfo)handle.TypeHandle.ToMethodBase(handle.MethodHandle)!);
		}

		internal static IReadOnlyDictionary<(RuntimeMethodHandle, RuntimeTypeHandle), ConstructorMember> Cache { get; }

		internal ConstructorMember(ConstructorInfo constructorInfo)
			: base(constructorInfo)
		{
			this.Handle = constructorInfo.MethodHandle;
			this.Method = constructorInfo.ToDelegate();
			this.Parameters = constructorInfo.GetParameters().To(parameter => new MethodParameter(constructorInfo.MethodHandle, parameter)).ToImmutableArray();

			this._Create = constructorInfo.ToCreateType();
		}

		private readonly CreateType _Create;

		public RuntimeMethodHandle Handle { get; }

		public Delegate? Method { get; }

		public IImmutableList<MethodParameter> Parameters { get; }

		public new TypeMember Type => base.Type!;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ConstructorInfo(ConstructorMember member)
			=> (ConstructorInfo)member.Type.Handle.ToMethodBase(member.Handle)!;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public object Create(params object?[]? arguments)
			=> this._Create(arguments);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(MethodMember? other)
			=> this.Handle == other?.Handle;
	}
}
