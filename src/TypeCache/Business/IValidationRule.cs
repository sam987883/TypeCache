// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;

namespace TypeCache.Business;

public interface IValidationRule<in REQUEST>
{
	IEnumerable<string> Validate(REQUEST request);
}
