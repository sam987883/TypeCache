// Copyright (c) 2020 Samuel Abraham

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace sam987883.Common
{
	internal sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T>
	{
		private readonly CustomEqualityComparer<T> _EqualityComparer;

		public ReferenceEqualityComparer() =>
			this._EqualityComparer = new CustomEqualityComparer<T>((x, y) => ReferenceEquals(x, y));

		bool IEqualityComparer<T>.Equals([AllowNull] T x, [AllowNull] T y) =>
			this._EqualityComparer.Equals(x, y);

		int IEqualityComparer<T>.GetHashCode([DisallowNull] T value) =>
			this._EqualityComparer.GetHashCode(value);
	}
}
