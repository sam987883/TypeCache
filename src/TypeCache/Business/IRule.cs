// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;

namespace TypeCache.Business
{
	public interface IRule<in T, R>
	{
		ValueTask<R> ApplyAsync(T request, CancellationToken cancellationToken);
	}

	public interface IRule<in M, in T, R>
	{
		ValueTask<R> ApplyAsync(M metadata, T request, CancellationToken cancellationToken);
	}
}
