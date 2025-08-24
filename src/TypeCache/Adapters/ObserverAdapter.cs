// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Adapters;

public sealed class ObserverAdapter<T>(IObserver<object?> observer) : IObserver<T>
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public void OnCompleted()
		=> observer.OnCompleted();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public void OnError(Exception error)
		=> observer.OnError(error);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public void OnNext(T value)
		=> observer.OnNext(value);
}
