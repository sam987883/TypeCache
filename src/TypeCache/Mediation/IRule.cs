// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public interface IRule<in REQUEST>
	where REQUEST : IRequest
{
	Task ExecuteAsync(REQUEST request, CancellationToken token = default);
}

public interface IRule<in REQUEST, RESPONSE>
	where REQUEST : IRequest<RESPONSE>
{
	Task<RESPONSE> MapAsync(REQUEST request, CancellationToken token = default);
}
