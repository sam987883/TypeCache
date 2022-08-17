// Copyright (c) 2021 Samuel Abraham

using TypeCache.Business;
using TypeCache.Collections;

namespace TypeCache.GraphQL.SqlApi;

#nullable disable

public class SqlApiMetadata
{
	public string[] Columns { get; set; }

	public string DataSource { get; set; }

	public IMediator Mediator { get; set; }

	public string Table { get; set; }
}
