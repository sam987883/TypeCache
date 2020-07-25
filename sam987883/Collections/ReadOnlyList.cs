// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;
using static sam987883.Common.Extensions.IEnumerableExtensions;

namespace sam987883.Collections
{
	public class ReadOnlyList<T> : ReadOnlyCollection<T>, IReadOnlyList<T>
    {
		private readonly Func<int, T> _GetValue;

		public ReadOnlyList(IList<T> list) : base(list) =>
			this._GetValue = index => list[index];

		public ReadOnlyList(IEnumerable<T> enumerable) : this(enumerable.ToList()) { }

		public T this[int index] =>
			this._GetValue(index);
	}
}
