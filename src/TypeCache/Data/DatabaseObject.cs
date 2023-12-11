﻿// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;

namespace TypeCache.Data;

public readonly struct DatabaseObject(string name) : IEquatable<DatabaseObject>
{
	public string Name { get; } = name;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool Equals(DatabaseObject other)
		=> this.Name.Is(other.Name);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override int GetHashCode()
		=> this.Name.GetHashCode();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override string ToString()
		=> this.Name;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static implicit operator string(DatabaseObject _)
		=> _.Name;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static implicit operator ReadOnlySpan<char>(DatabaseObject _)
		=> _.Name;
}
