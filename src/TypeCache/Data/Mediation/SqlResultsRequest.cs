// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Data.Mediation;

public readonly record struct SqlResultsRequest(Type ModelType, SqlCommand Command, int ListInitialCapacity = 0);

public readonly record struct SqlResultsRequest<T>(SqlCommand Command, int ListInitialCapacity)
	where T : notnull, new();
