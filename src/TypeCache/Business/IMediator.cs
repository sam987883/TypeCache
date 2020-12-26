// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;

namespace TypeCache.Business
{
	public interface IMediator
	{
		ValueTask<Response<R>> ApplyRule<T, R>(T request, Func<T, R> rule, CancellationToken cancellationToken = default);

		ValueTask<Response<R>> ApplyRuleAsync<T, R>(T request, CancellationToken cancellationToken = default);

		ValueTask<Response<R>> ApplyRuleAsync<M, T, R>(M metadata, T request, CancellationToken cancellationToken = default);

		ValueTask<Response<R[]>> ApplyRulesAsync<T, R>(T request, CancellationToken cancellationToken = default);

		ValueTask<Response<R[]>> ApplyRulesAsync<M, T, R>(M metadata, T request, CancellationToken cancellationToken = default);

		ValueTask RunProcessAsync<T>(T request, CancellationToken cancellationToken = default);

		ValueTask RunProcessAsync<M, T>(M metadata, T request, CancellationToken cancellationToken = default);
	}
}
