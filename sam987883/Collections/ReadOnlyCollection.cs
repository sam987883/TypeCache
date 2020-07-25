// Copyright (c) 2020 Samuel Abraham

using System.Collections.Generic;
using static sam987883.Common.Extensions.IEnumerableExtensions;

namespace sam987883.Collections
{
	public class ReadOnlyCollection<T> : Enumerable<T>, IReadOnlyCollection<T>
	{
		private readonly int _Count;
		
		public ReadOnlyCollection(ICollection<T> collection) : base(collection) =>
			this._Count = collection.Count;

		public ReadOnlyCollection(IReadOnlyCollection<T> collection) : base(collection) =>
			this._Count = collection.Count;

		public ReadOnlyCollection(IEnumerable<T> enumerable) : base(enumerable) =>
			this._Count = enumerable.Count();

		public int Count =>
			this._Count;
	}
}
