// Copyright (c) 2021 Samuel Abraham

using System.Data;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

public class SqlDataTableRequest : IRequest<DataTable>
{
	public required SqlCommand Command { get; set; }
}
