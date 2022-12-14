// Copyright (c) 2021 Samuel Abraham

using System.Data;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

public sealed class SqlDataTableRequest : IRequest<DataTable>
{
	public required SqlCommand Command { get; set; }
}
