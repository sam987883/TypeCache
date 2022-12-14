// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public interface IValidationRule<in REQUEST>
{
	IEnumerable<string> Validate(REQUEST request);
}
