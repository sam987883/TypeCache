// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;

namespace TypeCache.Business
{
	public interface IRuleIntermediary<in I, O>
	{
		ValueTask<O> HandleAsync(I request, CancellationToken cancellationToken = default);
	}
}
