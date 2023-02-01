// Copyright (c) 2021 Samuel Abraham

using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

public sealed record class SqlExecuteRequest(SqlCommand Command) : IRequest
{
}
