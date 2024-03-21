// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public interface IRule<in REQUEST>
	where REQUEST : IRequest
{
	Task Execute(REQUEST request, CancellationToken token = default);
}

public interface IRule<in REQUEST, RESPONSE>
	where REQUEST : IRequest<RESPONSE>
{
	Task<RESPONSE> Map(REQUEST request, CancellationToken token = default);
}
