// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;

namespace TypeCache.Business
{
	public interface IProcessIntermediary<in I>
	{
		ValueTask HandleAsync(I request, CancellationToken cancellationToken = default);
	}
}
