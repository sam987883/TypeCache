﻿// Copyright (c) 2021 Samuel Abraham

using GraphQL.Server.Ui.Playground;
using Microsoft.Data.SqlClient;
using TypeCache.Attributes;
using TypeCache.Data;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.TestApp.Models;
using TypeCache.GraphQL.TestApp.Tables;
using static System.Math;

const string DATASOURCE = "Default";

try
{
	var appBuilder = WebApplication.CreateBuilder(args);
	appBuilder.Services
		.AddMediation()
		.AddSqlCommandRules()
		.AddHashMaker((decimal)(Tau - E), (decimal)(Tau + 2 * E))
		.AddDataSource(DATASOURCE, SqlClientFactory.Instance, appBuilder.Configuration.GetConnectionString(DATASOURCE)!, ["AdventureWorks2019"])
		.AddGraphQL()
		.AddGraphQLTypeExtensions<Person>(person =>
		{
			person.AddField<Detail>(typeof(Detail).GetMethod(nameof(Detail.GetDetail))!);
			person.AddField<Detail>(typeof(Detail).GetMethod(nameof(Detail.GetDetails))!, (person, details) => details);
		});

	var app = appBuilder.Build();
	app
		.UseRouting()
		.UseGraphQLSchema("/graphql/custom", (schema, provider) =>
		{
			var dataSource = provider.GetRequiredKeyedService<IDataSource>(DATASOURCE);

			schema.AddVersion("1.0");
			schema.AddSqlApiEndpoints<Person>(dataSource, "Person.Person");
			schema.AddSqlApiEndpoints<Product>(dataSource, "Production.Product");
			schema.AddSqlApiEndpoints<WorkOrder>(dataSource, "Production.WorkOrder");
		})
		.UseGraphQLSchema("/graphql/data", (schema, provider) =>
		{
			var dataSource = provider.GetRequiredKeyedService<IDataSource>(DATASOURCE);

			schema.AddVersion("1.0");
			schema.AddDatabaseEndpoints(dataSource, SqlApiAction.CRUD, "AdventureWorks2019", "Person");
		})
		.UseGraphQLSchema("/graphql/schema", (schema, provider) =>
		{
			var dataSource = provider.GetRequiredKeyedService<IDataSource>(DATASOURCE);

			schema.AddVersion("1.0");
			schema.AddDatabaseSchemaQueries(dataSource);
		})
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
}
catch (Exception ex)
{
	Console.WriteLine(ex.ToString());
}
