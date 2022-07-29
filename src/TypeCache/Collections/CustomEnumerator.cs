// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static TypeCache.Default;

namespace TypeCache.Collections;

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

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool MoveNext()
		=> this.MoveNextFunc();

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public void Reset()
		=> throw new NotImplementedException();
}
