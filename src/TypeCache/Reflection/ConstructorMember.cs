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
			Cache = new LazyDictionary<RuntimeMethodHandle, ConstructorMember>(handle => new ConstructorMember((ConstructorInfo)handle.ToMethodBase()!));
		}

		internal static IReadOnlyDictionary<RuntimeMethodHandle, ConstructorMember> Cache { get; }

		internal ConstructorMember(ConstructorInfo constructorInfo)
			: base(constructorInfo, constructorInfo.IsAssembly, constructorInfo.IsPublic)
		{
			this.Create = constructorInfo.ToCreateType();
			this.Handle = constructorInfo.MethodHandle;
			this.Method = constructorInfo.ToDelegate();
			this.Parameters = constructorInfo.GetParameters().To(parameter => new MethodParameter(constructorInfo.MethodHandle, parameter)).ToImmutableArray();
			this.Type = constructorInfo.GetTypeMember();
		}

		public CreateType? Create { get; }

		public RuntimeMethodHandle Handle { get; }

		public Delegate? Method { get; }

		public IImmutableList<MethodParameter> Parameters { get; }

		public TypeMember Type { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(MethodMember? other)
			=> this.Handle == other?.Handle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this.Handle.GetHashCode();
	}
}
