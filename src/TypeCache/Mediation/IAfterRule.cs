// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public interface IAfterRule<in REQUEST>
	where REQUEST : notnull
{
	Task Handle(REQUEST request, CancellationToken token = default);
}
