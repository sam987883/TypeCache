// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;

namespace TypeCache.Business
{
	public interface IProcessHandler<in T>
	{
		ValueTask HandleAsync(T request, CancellationToken cancellationToken);
	}

	public interface IProcessHandler<in M, in T>
	{
		ValueTask HandleAsync(M metadata, T request, CancellationToken cancellationToken);
	}
}
