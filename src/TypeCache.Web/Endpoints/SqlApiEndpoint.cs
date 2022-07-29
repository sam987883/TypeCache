// Copyright (c) 2021 Samuel Abraham

using TypeCache.Business;

namespace TypeCache.Web.Endpoints;

public abstract class SqlApiEndpoint : WebApiEndpoint
{
	public SqlApiEndpoint(IMediator mediator, string dataSource)
		: base(mediator)
	{
		this.DataSource = dataSource;
	}

	protected string DataSource { get; }
}
