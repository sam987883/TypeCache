// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;

namespace TypeCache.Collections
{
	public abstract class CustomEquatable<T> : IEquatable<CustomEquatable<T>>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(CustomEquatable<T>? other)
			=> other is not null && this.EqualityFactors.ToHashSet().SetEquals(other.EqualityFactors);

		/// <summary>
		/// Override this and return the field or property values to use when considering equality.
		/// </summary>
		protected abstract IEnumerable<object> EqualityFactors { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? other)
			=> other is T item && this.Equals(item);

		public override int GetHashCode()
			=> unchecked(this.EqualityFactors.To(factor => factor?.GetHashCode() ?? 0).Aggregate(17, (current, value) => current * 23 + value));
	}
}
