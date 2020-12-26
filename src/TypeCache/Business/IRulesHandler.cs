// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;

namespace TypeCache.Business
{
	public interface IRulesHandler<in T, R>
	{
		ValueTask<Response<R[]>> HandleAsync(T request, CancellationToken cancellationToken);
	}

	public interface IRulesHandler<in M, in T, R>
	{
		ValueTask<Response<R[]>> HandleAsync(M metadata, T request, CancellationToken cancellationToken);
	}
}
