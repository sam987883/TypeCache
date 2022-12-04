// Copyright (c) 2021 Samuel Abraham

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

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool MoveNext()
		=> this.MoveNextFunc();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public void Reset()
		=> throw new NotImplementedException();
}
