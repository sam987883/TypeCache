// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Data;

public readonly record struct DatabaseObject(string Name) : IEquatable<DatabaseObject>
{
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Equals(DatabaseObject other)
		=> this.Name.Is(other.Name);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> this.Name.GetHashCode();

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override string ToString()
		=> this.Name;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static implicit operator string(DatabaseObject _)
		=> _.Name;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static implicit operator ReadOnlySpan<char>(DatabaseObject _)
		=> _.Name;
}
