// Copyright (c) 2021 Samuel Abraham

using System.Text.Json.Nodes;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

public class SqlJsonArrayRequest : IRequest<JsonArray>
{
	public required SqlCommand Command { get; set; }
}
