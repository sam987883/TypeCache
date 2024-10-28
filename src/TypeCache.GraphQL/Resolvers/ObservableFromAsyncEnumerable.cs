using System.Runtime.CompilerServices;
using System.Security.Claims;
using GraphQL;
using GraphQL.Execution;
using GraphQL.Instrumentation;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQLParser.AST;

namespace TypeCache.GraphQL.Resolvers;

/// <inheritdoc cref="ObservableFromAsyncEnumerable{T}.Create(Func{IResolveFieldContext, IAsyncEnumerable{T}})"/>
internal sealed class ObservableFromAsyncEnumerable<T> : IObservable<object?>, IDisposable
{
	private readonly CancellationTokenSource _cancellationTokenSource;
	private IAsyncEnumerable<T>? _enumerable;

	private ObservableFromAsyncEnumerable(CancellationTokenSource cts, IAsyncEnumerable<T> enumerable)
	{
		_cancellationTokenSource = cts;
		_enumerable = enumerable;
	}

	/// <summary>
	/// Returns a source stream resolver delegate (which returns an <see cref="IObservable{T}"/>) from a delegate
	/// which returns <see cref="IAsyncEnumerable{T}"/>. Each execution will create a new
	/// <see cref="ObservableFromAsyncEnumerable{T}"/> instance which can only be used once.
	/// </summary>
	public static Func<IResolveFieldContext, ValueTask<IObservable<object?>>> Create(Func<IResolveFieldContext, ValueTask<IAsyncEnumerable<T>>> func)
	{
		return async context =>
		{
			var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);

			// The underlying cancellation token is signaled if the HTTP connection is aborted. However, the
			// cts here may be signalled if the specific subscription is stopped within a websocket
			// connection. Hence a linked token is created and passed into the IResolveFieldContext
			// which is used to populate the arguments of the delegate.

			((ResolveFieldContext)context).CancellationToken = tokenSource.Token;
			var enumerable = await func(context);
			return new ObservableFromAsyncEnumerable<T>(tokenSource, enumerable);
		};
	}

	/// <inheritdoc cref="Create(Func{IResolveFieldContext, ValueTask{IAsyncEnumerable{T}}})"/>
	public static Func<IResolveFieldContext, ValueTask<IObservable<object?>>> Create(Func<IResolveFieldContext, Task<IAsyncEnumerable<T>>> func)
		=> Create(context => new ValueTask<IAsyncEnumerable<T>>(func(context)));

	/// <inheritdoc cref="Create(Func{IResolveFieldContext, ValueTask{IAsyncEnumerable{T}}})"/>
	public static Func<IResolveFieldContext, ValueTask<IObservable<object?>>> Create(Func<IResolveFieldContext, IAsyncEnumerable<T>> func)
		=> Create(context => new ValueTask<IAsyncEnumerable<T>>(func(context)));

	/// <inheritdoc/>
	public IDisposable Subscribe(IObserver<object?> observer)
	{
		// note: this error should not occur, as GraphQL.NET does not call IObservable<T>.Subscribe
		// more than once after executing the source stream resolver
		var enumerable = Interlocked.Exchange(ref _enumerable, null)
			?? throw new InvalidOperationException("This method can only be called once.");

		// this would only occur if IResolveFieldContext.CancellationToken has been signaled
		_cancellationTokenSource.Token.ThrowIfCancellationRequested();

		// iterate the async enumerable until the cancellation token is signaled via IDisposable.Dispose
		subscribe(observer, enumerable);

		return this;

		async void subscribe(IObserver<object?> observer, IAsyncEnumerable<T> enumerable)
		{
			try
			{
				// enumerate the source and pass the items to the observer
				await foreach (var item in enumerable.WithCancellation(_cancellationTokenSource.Token))
				{
					observer.OnNext(item);

					if (_cancellationTokenSource.IsCancellationRequested)
						return;
				}

				observer.OnCompleted();
			}
			catch (Exception ex)
			{
				// may occur within the source enumerable, or within the observer
				if (!_cancellationTokenSource.Token.IsCancellationRequested)
				{
					observer.OnError(ex);
				}
			}
		}
	}

	/// <summary>
	/// Signals the iteration task to cancel execution.
	/// </summary>
	void IDisposable.Dispose()
	{
		if (!_cancellationTokenSource.IsCancellationRequested)
			_cancellationTokenSource.Cancel();
	}
}
