using System;
using System.Linq;
using System.Threading;
using TypeCache.Extensions;

namespace TypeCache.Utilities;

/// <summary>
/// <see cref="IObservable{T}"> adapter for <see cref="IAsyncEnumerable{T}"/>.
/// </summary>
public sealed class ObservableAdapter<T>
	: IObservable<object?>, IDisposable
{
	private IAsyncEnumerable<T> _Enumerable;
	private readonly CancellationTokenSource _CancellationTokenSource;

	public ObservableAdapter(IAsyncEnumerable<T> enumerable, CancellationTokenSource cancellationTokenSource)
	{
		enumerable.ThrowIfNull();
		cancellationTokenSource.ThrowIfNull();

		this._Enumerable = enumerable;
		this._CancellationTokenSource = cancellationTokenSource;
	}

	public IDisposable Subscribe(IObserver<object?> observer)
	{
		this._Subscribe(observer);

		return this;

	}

	async void _Subscribe(IObserver<object?> observer)
	{
		try
		{
			var items = Interlocked.Exchange(ref this._Enumerable!, null)
				?? throw new InvalidOperationException("This Subscribe method can only be called once.");

			if (this._CancellationTokenSource.IsCancellationRequested)
				return;

			await foreach (var item in items.WithCancellation(this._CancellationTokenSource.Token))
			{
				observer.OnNext(item);

				if (this._CancellationTokenSource.IsCancellationRequested)
					return;
			}

			observer.OnCompleted();
		}
		catch (Exception ex)
		{
			if (!this._CancellationTokenSource.IsCancellationRequested)
				observer.OnError(ex);
		}
	}

	public void Dispose()
	{
		if (!this._CancellationTokenSource.IsCancellationRequested)
			this._CancellationTokenSource.Cancel();

		this._CancellationTokenSource.Dispose();
	}
}
