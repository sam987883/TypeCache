// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public interface IAfterRule<in REQUEST>
	where REQUEST : IRequest
{
	Task HandleAsync(REQUEST request, CancellationToken token = default);
}

public interface IAfterRule<in REQUEST, in RESPONSE>
	where REQUEST : IRequest<RESPONSE>
{
	Task HandleAsync(REQUEST request, RESPONSE response, CancellationToken token = default);
}
