// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Business;

public interface IProcess<in T>
{
	ValueTask PublishAsync(T request, CancellationToken token = default);
}
