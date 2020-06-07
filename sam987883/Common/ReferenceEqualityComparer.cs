// Copyright (c) 2020 Samuel Abraham

using System.Collections.Generic;

namespace sam987883.Common
{
	internal sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T>
	{
		private readonly CustomEqualityComparer<T> _EqualityComparer;

		public ReferenceEqualityComparer() =>
			this._EqualityComparer = new CustomEqualityComparer<T>((x, y) => ReferenceEquals(x, y));

		bool IEqualityComparer<T>.Equals(T x, T y) =>
			this._EqualityComparer.Equals(x, y);

		int IEqualityComparer<T>.GetHashCode(T value) =>
			this._EqualityComparer.GetHashCode(value);
	}
}
