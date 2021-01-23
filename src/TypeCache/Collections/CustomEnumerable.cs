// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TypeCache.Collections
{
	/// <summary>
	/// Provides faster iteration.
	/// </summary>
	public sealed class CustomEnumerable<T>
	{
		private readonly IEnumerable<T> _Enumerable;

		public CustomEnumerable(IEnumerable<T> enumerable)
		{
			this._Enumerable = enumerable;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerator<T> GetEnumerator()
			=> this._Enumerable.GetEnumerator();

		public int Count()
		{
			var count = 0;
			using var enumerator = this.GetEnumerator();
			while (enumerator.MoveNext())
				++count;
			return count;
		}

		public T? First()
		{
			using var enumerator = this.GetEnumerator();
			return enumerator.MoveNext() ? enumerator.Current : default;
		}

		public T? Get(Index index)
		{
			var i = index.Value;
			using var enumerator = this.GetEnumerator();
			while (i > 0 && enumerator.MoveNext())
				--i;
			return i == 0 ? enumerator.Current : default;
		}

		public bool Has(Index index)
		{
			using var enumerator = this.GetEnumerator();
			var i = index.Value;
			while (i > 0 && enumerator.MoveNext())
				--i;
			return i == 0;
		}

		public T? Move(ref int count)
		{
			using var enumerator = this.GetEnumerator();
			while (count > 0 && enumerator.MoveNext())
				--count;

			return count == 0 ? enumerator.Current : default;
		}
	}
}
