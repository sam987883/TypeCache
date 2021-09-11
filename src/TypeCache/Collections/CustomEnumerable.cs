// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Collections
{
	public readonly struct CustomEnumerable<T> : IEnumerable<T>
	{
		private readonly Func<IEnumerator<T>> _GetEnumerator;

		/// <exception cref="ArgumentNullException"/>
		public CustomEnumerable(Func<IEnumerator<T>> getEnumerator)
		{
			getEnumerator.AssertNotNull(nameof(getEnumerator));

			this._GetEnumerator = getEnumerator;
		}

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public IEnumerator<T> GetEnumerator()
			=> this._GetEnumerator();

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		IEnumerator IEnumerable.GetEnumerator()
			=> this._GetEnumerator();
	}
}
