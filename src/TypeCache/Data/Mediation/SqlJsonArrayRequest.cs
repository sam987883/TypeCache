// Copyright (c) 2021 Samuel Abraham

using System.Text.Json.Nodes;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

public sealed record class SqlJsonArrayRequest(SqlCommand Command) : IRequest<JsonArray>
{
}
