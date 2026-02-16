// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public interface IRuleHandler<REQUEST, RESPONSE>
	where REQUEST : notnull
{
	RESPONSE Handle(REQUEST request, CancellationToken token, Func<REQUEST, CancellationToken, RESPONSE> next);
}
