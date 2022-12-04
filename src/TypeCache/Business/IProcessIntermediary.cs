// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Business;

public interface IProcessIntermediary<in REQUEST>
{
	ValueTask RunAsync(REQUEST request, Action onComplete, CancellationToken token = default);
}
