// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;

namespace TypeCache.Business
{
	public interface IProcess<in T>
	{
		ValueTask RunAsync(T request, CancellationToken cancellationToken);
	}

	public interface IProcess<in M, in T>
	{
		ValueTask RunAsync(M metadata, T request, CancellationToken cancellationToken);
	}
}
