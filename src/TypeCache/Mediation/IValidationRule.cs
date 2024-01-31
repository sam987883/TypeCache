// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public interface IValidationRule<in REQUEST>
{
	Task Validate(REQUEST request, CancellationToken token = default);
}
