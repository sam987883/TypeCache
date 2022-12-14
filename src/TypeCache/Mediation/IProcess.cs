// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public interface IProcess<in REQUEST>
	where REQUEST : IRequest
{
	ValueTask PublishAsync(REQUEST request, CancellationToken token = default);
}
