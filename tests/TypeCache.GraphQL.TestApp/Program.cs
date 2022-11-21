// Copyright (c) 2021 Samuel Abraham

using GraphQL.Server.Ui.Playground;
using Microsoft.Data.SqlClient;
using TypeCache.Attributes;
using TypeCache.Data;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.TestApp.Tables;
using static System.Math;

const string DATASOURCE = "Default";

var builder = WebApplication.CreateBuilder(args);
builder.Services
	.RegisterMediator()
	.RegisterHashMaker((decimal)(Tau - E), (decimal)(Tau + 2*E))
	.RegisterDataSource(DATASOURCE, SqlClientFactory.Instance, builder.Configuration.GetConnectionString(DATASOURCE)!, DataSourceType.SqlServer)
	.RegisterDataSourceAccessor()
	.RegisterSqlCommandRules()
	.RegisterSqlApiRules(typeof(Program).Assembly)
	.RegisterGraphQL()
	.RegisterGraphQLSchema("AdventureWorks2019Data", schema =>
	{
		schema.AddVersion("1.0");
		schema.AddDatabaseEndpoints(DATASOURCE, SqlApiAction.CRUD, "AdventureWorks2019", "Person");
	})
	//.RegisterGraphQLSchema("AdventureWorks2019Data", schema =>
	//{
	//	schema.AddVersion("1.0");
	//	schema.AddSqlApiEndpoints<Person>(DATASOURCE, "Person.Person");
	//	schema.AddSqlApiEndpoints<Product>(DATASOURCE, "Production.Product");
	//	schema.AddSqlApiEndpoints<WorkOrder>(DATASOURCE, "Production.WorkOrder");
	//})
	.RegisterGraphQLSchema("AdventureWorks2019Schema", schema =>
	{
		schema.AddVersion("1.0");
		schema.AddDatabaseSchemaQueries(DATASOURCE);
	});

var app = builder.Build();
app
	.UseRouting()
	.UseGraphQLSchema("AdventureWorks2019Data", "/graphql/data")
	.UseGraphQLSchema("AdventureWorks2019Schema", "/graphql/schema")
	.UseGraphQLPlayground("/playground", new()
	{
		BetaUpdates = true,
		EditorCursorShape = EditorCursorShape.Line,
		EditorFontFamily = "Lucida Console",
		EditorFontSize = 12,
		EditorReuseHeaders = true,
		EditorTheme = EditorTheme.Dark,
		HideTracingResponse = true,
		PrettierTabWidth = 2,
		SchemaPollingEnabled = true,
		SchemaPollingInterval = 10000
	});

await app.RunAsync();
