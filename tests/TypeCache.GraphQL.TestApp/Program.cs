// Copyright (c) 2021 Samuel Abraham

using System.Text;
using System.Text.Json.Serialization;
using GraphiQl;
using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Models;
using TypeCache.Attributes;
using TypeCache.Data;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.TestApp.Models;
using TypeCache.GraphQL.TestApp.Tables;
using TypeCache.Web.Extensions;

const string DATASOURCE = "Default";

try
{
	var stringEnumJsonConverter = new JsonStringEnumConverter();
	var appBuilder = WebApplication.CreateBuilder(args);
	appBuilder.Services
		.ConfigureHttpJsonOptions(_ => _.SerializerOptions.Converters.Add(stringEnumJsonConverter))
		.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(_ => _.JsonSerializerOptions.Converters.Add(stringEnumJsonConverter))
		.AddMediation()
		.AddSqlCommandRules()
		.AddHashMaker(Encoding.UTF8.GetBytes("ABCDEFghijklMNOP"))
		.AddDataSource(DATASOURCE, SqlClientFactory.Instance, appBuilder.Configuration.GetConnectionString(DATASOURCE)!, ["AdventureWorks2019"])
		.AddGraphQL()
		.AddGraphQLTypeExtensions<Person>(person =>
		{
			person.AddField<Detail>(typeof(Detail).GetMethod(nameof(Detail.GetDetail))!);
			person.AddField<Detail>(typeof(Detail).GetMethod(nameof(Detail.GetDetails))!, (person, details) => details);
		})
		//.AddOpenApi()
		.AddEndpointsApiExplorer()
		.AddSwaggerGen(_ =>
		{
			_.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "SQL API",
				Version = "v1",
				Description = "An ASP.NET Core Web API with SQL endpoints",
				TermsOfService = new Uri("http://licenses.nuget.org/MIT"),
				Contact = new OpenApiContact
				{
					Name = "Samuel Abraham",
					Email = "sam987883@gmail.com",
					Url = new Uri("https://www.nuget.org/packages/TypeCache.Web/")
				},
				License = new OpenApiLicense
				{
					Name = "License",
					Url = new Uri("http://licenses.nuget.org/MIT")
				}
			});
			_.EnableAnnotations();
		});

	var app = appBuilder.Build();
	app
		.UseRouting()
		.UseEndpoints(_ => _.MapSqlApi().WithOpenApi())
		.UseEndpoints(_ => _.MapSqlGet().WithOpenApi())
		.UseSwagger()
		.UseSwaggerUI(_ =>
		{
			_.SwaggerEndpoint("/swagger/v1/swagger.json", "SQL API v1");
			//_.RoutePrefix = string.Empty;
		})
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
		.UseGraphiQl("/playground/custom", "/graphql/custom")
		.UseGraphiQl("/playground/data", "/graphql/data")
		.UseGraphiQl("/playground/schema", "/graphql/schema");

	await app.RunAsync();
}
catch (Exception ex)
{
	Console.WriteLine(ex.ToString());
}
