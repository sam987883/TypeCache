// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace sam987883.Collections
{
	public class Enumerable<T> : IEnumerable<T>
	{
		private readonly Func<IEnumerator<T>> _GetEnumerator;

		public Enumerable(Func<IEnumerator<T>> getEnumerator) =>
			this._GetEnumerator = getEnumerator;

		public Enumerable(IEnumerable<T> enumerable) =>
			this._GetEnumerator = enumerable.GetEnumerator;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerator<T> GetEnumerator() =>
			this._GetEnumerator();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator() =>
			this._GetEnumerator();
	}
}
