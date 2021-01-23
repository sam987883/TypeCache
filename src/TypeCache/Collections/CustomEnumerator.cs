// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Collections
{
	public class CustomEnumerator<T> : IDisposable
	{
		private readonly IEnumerator<T> _Enumerator;

		public CustomEnumerator(IEnumerator<T> enumerator)
		{
			enumerator.AssertNotNull(nameof(IEnumerator<T>));

			this._Enumerator = enumerator;
		}

		public T Current => this._Enumerator.Current;

		public void Dispose()
		{
			(this._Enumerator as IDisposable)?.Dispose();
			GC.SuppressFinalize(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
			=> this._Enumerator.MoveNext();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
			=> throw new NotImplementedException();
	}
}
