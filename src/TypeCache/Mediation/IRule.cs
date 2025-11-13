// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public interface IRule<in REQUEST>
	where REQUEST : notnull
{
	Task Execute(REQUEST request, CancellationToken token = default);
}

public interface IRule<in REQUEST, RESPONSE>
	where REQUEST : notnull
{
	ValueTask<RESPONSE> Send(REQUEST request, CancellationToken token = default);
}
