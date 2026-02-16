// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public interface IRule<in REQUEST, out RESPONSE>
	where REQUEST : notnull
{
	RESPONSE Send(REQUEST request, CancellationToken token);
}
