// Copyright (c) 2021 Samuel Abraham

using System.Data;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

public sealed record class SqlDataTableRequest(SqlCommand Command) : IRequest<DataTable>
{
}
