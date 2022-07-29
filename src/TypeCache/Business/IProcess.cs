// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;

namespace TypeCache.Business;

public interface IProcess<in T>
{
	ValueTask PublishAsync(T request, CancellationToken token = default);
}
