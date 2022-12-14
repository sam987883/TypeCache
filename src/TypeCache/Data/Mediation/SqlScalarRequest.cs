// Copyright (c) 2021 Samuel Abraham

using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

public sealed class SqlScalarRequest : IRequest<object?>
{
	public required SqlCommand Command { get; set; }
}
