// Copyright (c) 2021 Samuel Abraham

using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

public sealed class SqlModelsRequest : IRequest<IList<object>>
{
	public required SqlCommand Command { get; set; }

	public int ListInitialCapacity { get; set; }

	public required Type ModelType { get; set; }
}
