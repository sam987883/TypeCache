// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using static TypeCache.Default;

namespace TypeCache.Collections;

public abstract class CustomEquatable<T> : IEquatable<CustomEquatable<T>>
{
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Equals(CustomEquatable<T>? other)
		=> other is not null && this.EqualityFactors.ToHashSet().SetEquals(other.EqualityFactors);

	/// <summary>
	/// Override this and return the field or property values to use when considering equality.
	/// </summary>
	protected abstract IEnumerable<object> EqualityFactors { get; }

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override bool Equals(object? other)
		=> other is T item && this.Equals(item);

	public override int GetHashCode()
	{
		var hashCode = new HashCode();
		this.EqualityFactors.Do(hashCode.Add);
		return hashCode.ToHashCode();
	}
}
