// Copyright (c) 2021 Samuel Abraham

using GraphQL.Server.Ui.Playground;
using Microsoft.Data.SqlClient;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.TestApp.Schemas;
using static System.Math;
using static TypeCache.Default;

const string DATABASE_PROVIDER = "Microsoft.Data.SqlClient";

SqlClientFactory.Instance.Register(DATABASE_PROVIDER);

var builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterMediator()
	.RegisterHashMaker((decimal)(Tau - E), (decimal)(Tau + 2*E))
	.RegisterDataSource(DATASOURCE, DATABASE_PROVIDER, builder.Configuration.GetConnectionString(DATASOURCE))
	.RegisterDataSourceAccessor()
	.RegisterSqlApiRules(typeof(Program).Assembly)
	.RegisterGraphQL()
	.AddSingleton<TestGraphQLSchema>();

var app = builder.Build();
app.UseRouting()
	.UseGraphQLSchema<TestGraphQLSchema>("/graphql")
	.UseGraphQLPlayground(new()
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
	}, "/playground");
app.Run();
