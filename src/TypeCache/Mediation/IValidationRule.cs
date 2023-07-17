// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public interface IValidationRule<in REQUEST>
{
	Task ValidateAsync(REQUEST request, CancellationToken token = default);
}
