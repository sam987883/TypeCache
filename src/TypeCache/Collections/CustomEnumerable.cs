// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TypeCache.Collections
{
	public sealed class CustomEnumerable<T>
	{
		private readonly IEnumerable<T> _Enumerable;

		public CustomEnumerable(IEnumerable<T> enumerable)
		{
			this._Enumerable = enumerable;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CustomEnumerator<T> GetEnumerator()
		{
			var enumerator = this._Enumerable.GetEnumerator();
			return new CustomEnumerator<T>
			{
				Enumerator = enumerator,
				CurrentFunc = () => enumerator.Current,
				MoveNextFunc = enumerator.MoveNext
			};
		}
	}
}
