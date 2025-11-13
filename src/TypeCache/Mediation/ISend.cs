// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public interface ISend<RESPONSE>
{
	ValueTask<RESPONSE> Send<REQUEST>(REQUEST request, CancellationToken token = default)
		where REQUEST : notnull;
}
