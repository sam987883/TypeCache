namespace TypeCache.Utilities;

/// <summary>
/// Converts an <see cref="IObservable{T}"/> for value types into an <see cref="IObservable{T}">IObservable&lt;object?&gt;</see>.
/// </summary>
public sealed class CustomObservable<T>(IObservable<T> observable) : IObservable<object?>
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public IDisposable Subscribe(IObserver<object?> observer)
		=> observable.Subscribe(new ObserverAdapter<T>(observer));
}
