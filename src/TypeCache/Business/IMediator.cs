// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;

namespace TypeCache.Business
{
	public interface IMediator
	{
		ValueTask<O> ApplyRulesAsync<I, O>(I request, CancellationToken cancellationToken = default);

		ValueTask RunProcessAsync<I>(I request, CancellationToken cancellationToken = default);
	}
}
