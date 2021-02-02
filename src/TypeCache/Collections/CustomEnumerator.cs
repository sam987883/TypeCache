// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Collections
{
	public readonly struct CustomEnumerator<T> : IDisposable
	{
		public IEnumerator<T> Enumerator { get; init; }

		public Func<T> CurrentFunc { get; init; }

		public Func<bool> MoveNextFunc { get; init; }

		public T Current => this.CurrentFunc();

		public void Dispose()
		{
			(this.Enumerator as IDisposable)?.Dispose();
			GC.SuppressFinalize(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
			=> this.MoveNextFunc();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
			=> throw new NotImplementedException();
	}
}
