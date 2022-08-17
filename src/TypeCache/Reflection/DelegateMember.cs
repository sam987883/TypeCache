// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

[DebuggerDisplay("Delegate {Name,nq}", Name = "{Name}")]
public class DelegateMember : Member, IEquatable<DelegateMember>
{
	private const string METHOD_NAME = "Invoke";

	private readonly Lazy<Func<object?[]?, object?>> _Invoke;
	private readonly Lazy<Delegate> _Method;

	internal DelegateMember(Type type) : base(type)
	{
		typeof(Delegate).IsAssignableFrom(type.BaseType).AssertEquals(true);

		this.Handle = type.TypeHandle;

		var methodInfo = type.GetMethod(METHOD_NAME, INSTANCE_BINDING_FLAGS)!;

		this.MethodHandle = methodInfo.MethodHandle;
		this.Parameters = methodInfo.GetParameters().Map(parameter => new MethodParameter(methodInfo.MethodHandle, parameter)).ToImmutableArray();
		this.Return = new ReturnParameter(methodInfo);
		this._Invoke = Lazy.Create(() => ((MethodInfo)this.MethodHandle.ToMethodBase(this.Handle)!).LambdaInvoke().Compile());
		this._Method = Lazy.Create(() => ((MethodInfo)this.MethodHandle.ToMethodBase(this.Handle)!).Lambda().Compile());
	}

	public RuntimeTypeHandle Handle { get; }

	public Delegate Method => this._Method.Value;

	public RuntimeMethodHandle MethodHandle { get; }

	public IImmutableList<MethodParameter> Parameters { get; }

	public ReturnParameter Return { get; }

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Equals([NotNullWhen(true)] DelegateMember? other)
		=> other is not null && this.Handle.Equals(other.Handle);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> this.Equals(item as DelegateMember);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> this.Handle.GetHashCode();

	/// <remarks>First item in <paramref name="arguments"/> must be the instance of the type that the methode belongs to, unless the method is <c><see langword="static"/></c>.</remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public object? Invoke(params object?[]? arguments)
		=> this._Invoke.Value(arguments);
}
