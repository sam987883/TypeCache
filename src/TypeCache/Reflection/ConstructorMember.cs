﻿// Copyright (c) 2021 Samuel Abraham

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
	public readonly struct ConstructorMember
		: IMember, IEquatable<ConstructorMember>
	{
		static ConstructorMember()
		{
			Cache = new LazyDictionary<(RuntimeMethodHandle, RuntimeTypeHandle), ConstructorMember>(CreateConstructorMember);

			static ConstructorMember CreateConstructorMember((RuntimeMethodHandle MethodHandle, RuntimeTypeHandle TypeHandle) handle)
				=> new ConstructorMember((ConstructorInfo)handle.TypeHandle.ToMethodBase(handle.MethodHandle)!);
		}

		internal static IReadOnlyDictionary<(RuntimeMethodHandle, RuntimeTypeHandle), ConstructorMember> Cache { get; }

		internal ConstructorMember(ConstructorInfo constructorInfo)
		{
			this.Type = constructorInfo.GetTypeMember();
			this.Attributes = constructorInfo.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
			this.Name = this.Attributes.First<NameAttribute>()?.Name ?? constructorInfo.Name;
			this.Handle = constructorInfo.MethodHandle;
			this.Method = constructorInfo.ToDelegate();
			this.Parameters = constructorInfo.GetParameters().To(parameter => new MethodParameter(constructorInfo.MethodHandle, parameter)).ToImmutableArray();
			this.Internal = constructorInfo.IsAssembly;
			this.Public = constructorInfo.IsPublic;

			this._Create = constructorInfo.ToCreateType();
		}

		private readonly CreateType _Create;

		public TypeMember Type { get; }

		public IImmutableList<Attribute> Attributes { get; }

		public string Name { get; }

		public RuntimeMethodHandle Handle { get; }

		public Delegate? Method { get; }

		public IImmutableList<MethodParameter> Parameters { get; }

		public bool Internal { get; }

		public bool Public { get; }

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static implicit operator ConstructorInfo(ConstructorMember member)
			=> (ConstructorInfo)member.Type.Handle.ToMethodBase(member.Handle)!;

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public object Create(params object?[]? arguments)
			=> this._Create(arguments);

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public bool Equals(ConstructorMember other)
			=> this.Handle == other.Handle;

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public override int GetHashCode()
			=> this.Handle.GetHashCode();
	}
}
