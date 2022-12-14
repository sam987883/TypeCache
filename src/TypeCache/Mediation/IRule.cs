// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public interface IRule<in REQUEST>
	where REQUEST : IRequest
{
	ValueTask ApplyAsync(REQUEST request, CancellationToken token = default);
}

public interface IRule<in REQUEST, RESPONSE>
	where REQUEST : IRequest<RESPONSE>
{
	ValueTask<RESPONSE> ApplyAsync(REQUEST request, CancellationToken token = default);
}
