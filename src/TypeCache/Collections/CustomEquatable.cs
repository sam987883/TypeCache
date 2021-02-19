// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;

namespace TypeCache.Collections
{
	public abstract class CustomEquatable<T> : IEquatable<T>
	{
		private readonly Func<T?, bool> _Equals;

		public CustomEquatable(Func<T?, bool> equals)
			=> this._Equals = equals;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(T? other)
			=> this._Equals(other);

		public override bool Equals(object? other)
			=> other switch { T item => item.Equals(this), _ => false };

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> base.GetHashCode();

		public static bool operator ==(CustomEquatable<T> a, CustomEquatable<T> b)
			=> a.Equals(b);

		public static bool operator !=(CustomEquatable<T> a, CustomEquatable<T> b)
			=> !a.Equals(b);
	}
}
