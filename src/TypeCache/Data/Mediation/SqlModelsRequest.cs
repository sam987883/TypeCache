// Copyright (c) 2021 Samuel Abraham

using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

public sealed record class SqlModelsRequest(SqlCommand Command, Type ModelType) : IRequest<IList<object>>
{
	public int ListInitialCapacity { get; set; }
}
