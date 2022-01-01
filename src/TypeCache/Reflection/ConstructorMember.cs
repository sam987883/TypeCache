// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

public class ConstructorMember : Member, IEquatable<ConstructorMember>
{
	private readonly CreateType _Create;

	internal ConstructorMember(ConstructorInfo constructorInfo, TypeMember type) : base(constructorInfo)
	{
		this.Type = type;
		this.Handle = constructorInfo.MethodHandle;
		this.Method = constructorInfo.Lambda().Compile();
		this.Parameters = constructorInfo.GetParameters().To(parameter => new MethodParameter(constructorInfo.MethodHandle, parameter)).ToImmutableArray();

		this._Create = constructorInfo.LambdaCreateType().Compile();
	}

	public RuntimeMethodHandle Handle { get; }

	public Delegate? Method { get; }

	public IImmutableList<MethodParameter> Parameters { get; }

	public TypeMember Type { get; }

	/// <summary>
	/// <c>=&gt; <see langword="this"/>._Create(<paramref name="arguments"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public object Create(params object?[]? arguments)
		=> this._Create(arguments);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public bool Equals(ConstructorMember? other)
		=> this.Handle == other?.Handle;

	/// <summary>
	/// <c>=&gt; (<see cref="ConstructorInfo"/>)<paramref name="member"/>.Type.Handle.ToMethodBase(<paramref name="member"/>.Handle)!;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static implicit operator ConstructorInfo(ConstructorMember member)
		=> (ConstructorInfo)member.Type.Handle.ToMethodBase(member.Handle)!;
}
