// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

[DebuggerDisplay("DelegateOf<{Name,nq}>", Name = "{Name}")]
public sealed class DelegateMember : IMember, IEquatable<DelegateMember>
{
	private const string METHOD_NAME = "Invoke";

	private readonly Lazy<Func<object?[]?, object?>> _Invoke;
	private readonly Lazy<Delegate> _Method;

	internal DelegateMember(Type type)
	{
		const BindingFlags INSTANCE_BINDING_FLAGS = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

		typeof(Delegate).IsAssignableFrom(type.BaseType).AssertTrue();

		this.Attributes = type.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
		this.Internal = !type.IsVisible;
		this.Name = type.Name();
		this.Public = type.IsPublic;
		this.TypeHandle = type.TypeHandle;

		var methodInfo = type.GetMethod(METHOD_NAME, INSTANCE_BINDING_FLAGS)!;

		this.MethodHandle = methodInfo.MethodHandle;
		this.Parameters = methodInfo.GetParameters().Select(parameter => new MethodParameter(methodInfo.MethodHandle, parameter)).ToImmutableArray();
		this.Return = new ReturnParameter(methodInfo);
		this._Invoke = Lazy.Create(() => ((MethodInfo)this.MethodHandle.ToMethodBase(this.TypeHandle)!).LambdaInvoke().Compile());
		this._Method = Lazy.Create(() => ((MethodInfo)this.MethodHandle.ToMethodBase(this.TypeHandle)!).Lambda().Compile());
	}

	/// <inheritdoc/>
	public IReadOnlyList<Attribute> Attributes { get; }

	public RuntimeTypeHandle TypeHandle { get; }

	/// <inheritdoc cref="Type.IsVisible"/>
	public bool Internal { get; }

	public Delegate Method => this._Method.Value;

	public RuntimeMethodHandle MethodHandle { get; }

	/// <inheritdoc/>
	public string Name { get; }

	public IImmutableList<MethodParameter> Parameters { get; }

	/// <inheritdoc cref="Type.IsPublic"/>
	public bool Public { get; }

	public ReturnParameter Return { get; }

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Equals([NotNullWhen(true)] DelegateMember? other)
		=> this.TypeHandle == other?.TypeHandle;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> this.Equals(item as DelegateMember);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> this.TypeHandle.GetHashCode();

	/// <remarks>FirstOrDefault item in <paramref name="arguments"/> must be the instance of the type that the methode belongs to, unless the method is <c><see langword="static"/></c>.</remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public object? Invoke(params object?[]? arguments)
		=> this._Invoke.Value(arguments);
}
