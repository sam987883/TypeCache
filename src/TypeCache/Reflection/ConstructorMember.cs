﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

[DebuggerDisplay("{Type,nq}.ctor", Name = "{Name}")]
public class ConstructorMember : Member, IEquatable<ConstructorMember>
{
	private readonly CreateType _Create;

	internal ConstructorMember(ConstructorInfo constructorInfo, TypeMember type) : base(constructorInfo)
	{
		this.Handle = constructorInfo.MethodHandle;
		this.Method = constructorInfo.Lambda().Compile();
		this.Parameters = constructorInfo.GetParameters().Map(parameter => new MethodParameter(constructorInfo.MethodHandle, parameter)).ToImmutableArray();

		this._Create = constructorInfo.LambdaCreateType().Compile();
	}

	public RuntimeMethodHandle Handle { get; }

	public Delegate? Method { get; }

	public IReadOnlyList<MethodParameter> Parameters { get; }

	/// <summary>
	/// <c>=&gt; <see langword="this"/>._Create(<paramref name="arguments"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public object Create(params object?[]? arguments)
		=> this._Create(arguments);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Equals(ConstructorMember? other)
		=> this.Handle == other?.Handle;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> item is ConstructorMember member && this.Equals(member);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> this.Handle.GetHashCode();

	/// <summary>
	/// <c>=&gt; (<see cref="ConstructorInfo"/>)<paramref name="member"/>.Handle.ToMethodBase(<paramref name="member"/>.Type.Handle)!;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static implicit operator ConstructorInfo(ConstructorMember member)
		=> (ConstructorInfo)member.Handle.ToMethodBase(member.Type!.Handle)!;
}
