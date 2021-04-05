// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Collections
{
	public sealed class CustomEnumerable<T> : IEnumerable<T>
	{
		public static IEnumerable<T> Empty
		{
			get { yield break; }
		}

		private readonly Func<IEnumerator<T>> _GetEnumerator;

		public CustomEnumerable(Func<IEnumerator<T>> getEnumerator)
		{
			getEnumerator.AssertNotNull(nameof(getEnumerator));
			this._GetEnumerator = getEnumerator;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerator<T> GetEnumerator()
			=> this._GetEnumerator();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator()
			=> this._GetEnumerator();
	}
}
