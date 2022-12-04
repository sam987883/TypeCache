// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Business;

public interface IRule<in REQUEST>
{
	ValueTask ApplyAsync(REQUEST request, CancellationToken token = default);
}

public interface IRule<in REQUEST, RESPONSE>
{
	ValueTask<RESPONSE> ApplyAsync(REQUEST request, CancellationToken token = default);
}
