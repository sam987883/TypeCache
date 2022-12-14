// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public interface IRuleIntermediary<in REQUEST>
	where REQUEST : IRequest
{
	ValueTask HandleAsync(REQUEST request, CancellationToken token = default);
}

public interface IRuleIntermediary<in REQUEST, RESPONSE>
	where REQUEST : IRequest<RESPONSE>
{
	ValueTask<RESPONSE> HandleAsync(REQUEST request, CancellationToken token = default);
}
