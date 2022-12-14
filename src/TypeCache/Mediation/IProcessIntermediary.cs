// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public interface IProcessIntermediary<in REQUEST>
	where REQUEST : IRequest
{
	ValueTask RunAsync(REQUEST request, Action onComplete, CancellationToken token = default);
}
