// Copyright (c) 2021 Samuel Abraham

using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using Microsoft.Data.SqlClient;
using TypeCache.Data;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.TestApp.Schemas;
using TypeCache.GraphQL.TestApp.Tables;
using static System.Math;
using static TypeCache.Default;

var builder = WebApplication.CreateBuilder(args);

var defaultDataSource = new DataSource
{
	Name = DATASOURCE,
	DatabaseProvider = "Microsoft.Data.SqlClient",
	ConnectionString = builder.Configuration.GetConnectionString(DATASOURCE),
};

builder.Services.RegisterMediator()
	.RegisterHashMaker((decimal)Tau, (decimal)E)
	.RegisterDatabaseProviderFactory(defaultDataSource.DatabaseProvider, SqlClientFactory.Instance)
	.RegisterSqlApi(defaultDataSource)
	.RegisterSqlApiRules()
	.RegisterGraphQL()
	.AddSingleton<TestGraphQLSchema>();

var app = builder.Build();

app.UseRouting();

app.UseGraphQLSchema<TestGraphQLSchema>("/graphql");

app.UseGraphQLPlayground(new()
{
	BetaUpdates = true,
	EditorCursorShape = EditorCursorShape.Block,
	//EditorFontFamily = "FixedSys",
	EditorFontSize = 12,
	EditorReuseHeaders = true,
	EditorTheme = EditorTheme.Dark,
	HideTracingResponse = true,
	PrettierTabWidth = 4,
	SchemaPollingEnabled = true,
	SchemaPollingInterval = 10000
}, "/playground");

app.Run();
