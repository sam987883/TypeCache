// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public interface IValidationRule<in REQUEST>
	where REQUEST : notnull
{
	void Validate(REQUEST request);
}
