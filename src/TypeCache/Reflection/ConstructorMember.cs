// Copyright (c) 2021 Samuel Abraham

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
	private readonly Lazy<Func<object?[]?, object?>> _Invoke;
	private readonly Lazy<Delegate> _Method;

	internal ConstructorMember(ConstructorInfo constructorInfo, TypeMember type) : base(constructorInfo)
	{
		this.Handle = constructorInfo.MethodHandle;
		this.Parameters = constructorInfo.GetParameters().Map(parameter => new MethodParameter(constructorInfo.MethodHandle, parameter)).ToImmutableArray();
		this.Type = type;
		this._Invoke = Lazy.Create(() => ((ConstructorInfo)this.Handle.ToMethodBase(this.Type.Handle)!).LambdaInvoke().Compile());
		this._Method = Lazy.Create(() => ((ConstructorInfo)this.Handle.ToMethodBase(this.Type.Handle)!).Lambda().Compile());
	}

	public RuntimeMethodHandle Handle { get; }

	public Delegate? Method => this._Method.Value;

	public IReadOnlyList<MethodParameter> Parameters { get; }

	/// <summary>
	/// The <see cref="TypeMember"/> that owns this <see cref="Member"/>.
	/// </summary>
	public TypeMember Type { get; }

	private Delegate CreateDelegate() => ((ConstructorInfo)this.Handle.ToMethodBase(this.Type.Handle)!).Lambda().Compile();

	private Func<object?[]?, object?> CreateInvoke() => ((ConstructorInfo)this.Handle.ToMethodBase(this.Type.Handle)!).LambdaInvoke().Compile();

	/// <summary>
	/// <c>=&gt; <see langword="this"/>._Invoke.Value(<paramref name="arguments"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public object Create(params object?[]? arguments)
		=> this._Invoke.Value(arguments)!;

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
