// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.GraphQL.TestApp.Tables;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.TestApp.Schemas;

public class TestGraphQLSchema : GraphQLSchema
{
	public TestGraphQLSchema(IServiceProvider provider) : base(provider)
	{
		this.Mutation = new ObjectGraphType { Name = nameof(this.Mutation) };
		this.Query = new ObjectGraphType { Name = nameof(this.Query) };

		this.AddVersion("1.0");
		this.AddSqlApiEndpoints<Person>("Default", "Person.Person");
		this.AddSqlApiEndpoints<Product>("Default", "Production.Product");
		this.AddSqlApiEndpoints<WorkOrder>("Default", "Production.WorkOrder");
	}
}
