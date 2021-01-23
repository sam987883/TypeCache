// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TypeCache.Collections
{
	public sealed class CustomComparer<T> : IComparer<T>, IEqualityComparer<T>
	{
		private readonly Comparison<T> _Compare;
		private readonly Func<T, T, bool> _Equals;
		private readonly Func<T, int> _GetHashCode;

		public CustomComparer(Comparison<T> compare, Func<T, T, bool> equals, Func<T, int> getHashCode)
		{
			this._Compare = compare;
			this._Equals = equals;
			this._GetHashCode = getHashCode;
		}

		public CustomComparer(Comparison<T> compare, Func<T, T, bool> equals)
			: this(compare, equals, _ => _.GetHashCode())
		{
		}

		public CustomComparer(Comparison<T> compare)
			: this(compare, (x, y) => compare(x, y) == 0, _ => _.GetHashCode())
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		int IComparer<T>.Compare([AllowNull] T x, [AllowNull] T y)
			=> this._Compare(x, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals([AllowNull] T x, [AllowNull] T y)
			=> this._Equals(x, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetHashCode([DisallowNull] T value)
			=> this._GetHashCode(value);
	}
}
