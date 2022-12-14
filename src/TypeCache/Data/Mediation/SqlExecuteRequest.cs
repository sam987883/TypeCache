// Copyright (c) 2021 Samuel Abraham

using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

public class SqlExecuteRequest : IRequest
{
	public required SqlCommand Command { get; set; }
}
