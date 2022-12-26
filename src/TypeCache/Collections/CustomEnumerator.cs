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
		var disposable = this.Enumerator as IDisposable;
		if (disposable is not null)
			disposable.Dispose();
		else
			GC.SuppressFinalize(this);
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool MoveNext()
		=> this.MoveNextFunc();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public void Reset()
		=> throw new NotImplementedException();
}
