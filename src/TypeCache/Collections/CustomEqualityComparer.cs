// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TypeCache.Collections
{
	public readonly struct CustomEqualityComparer<T> : IEqualityComparer<T>
	{
		private readonly Func<T?, T?, bool> _Equals;
		private readonly Func<T?, int> _GetHashCode;

		public CustomEqualityComparer(Func<T?, T?, bool> equals, Func<T?, int> getHashCode)
		{
			this._Equals = equals;
			this._GetHashCode = getHashCode;
		}

		public CustomEqualityComparer(Func<T?, T?, bool> equals) : this(equals, _ => _?.GetHashCode() ?? 0)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals([AllowNull] T? x, [AllowNull] T? y)
			=> this._Equals(x, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetHashCode([DisallowNull] T value)
			=> this._GetHashCode(value);
	}
}
