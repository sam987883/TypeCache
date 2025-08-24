// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;

namespace TypeCache.Adapters;

/// <summary>
/// <see cref="IObservable{T}"/> adapter for <see cref="IAsyncEnumerable{T}"/>.
/// </summary>
public sealed class ObservableAdapter<T>(IAsyncEnumerable<T> enumerable, CancellationTokenSource cancellationTokenSource)
	: IObservable<object?>, IDisposable
{
	public IDisposable Subscribe(IObserver<object?> observer)
	{
		this._Subscribe(observer);
		return this;
	}

	async void _Subscribe(IObserver<object?> observer)
	{
		enumerable.ThrowIfNull();
		cancellationTokenSource.ThrowIfNull();

		try
		{
			var items = Interlocked.Exchange(ref enumerable!, null)
				?? throw new InvalidOperationException("This Subscribe method can only be called once.");

			if (cancellationTokenSource.IsCancellationRequested)
				return;

			await foreach (var item in items.WithCancellation(cancellationTokenSource.Token))
			{
				observer.OnNext(item);

				if (cancellationTokenSource.IsCancellationRequested)
					return;
			}

			observer.OnCompleted();
		}
		catch (Exception ex)
		{
			if (!cancellationTokenSource.IsCancellationRequested)
				observer.OnError(ex);
		}
	}

	public void Dispose()
	{
		if (!cancellationTokenSource.IsCancellationRequested)
			cancellationTokenSource.Cancel();

		cancellationTokenSource.Dispose();
	}
}
