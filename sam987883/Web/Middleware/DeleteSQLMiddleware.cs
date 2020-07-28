// Copyright (c) 2020 Samuel Abraham

using Microsoft.AspNetCore.Http;
using sam987883.Common.Converters;
using sam987883.Database;
using sam987883.Database.Extensions;
using sam987883.Database.Models;
using sam987883.Dependencies;
using System;
using System.Data.Common;
using System.Text.Json;
using System.Threading.Tasks;

namespace sam987883.Web.Middleware
{
	public class DeleteSQLMiddleware
    {
        private readonly string _ConnectionString;
        private readonly DbProviderFactory _DbProviderFactory;

        public DeleteSQLMiddleware(RequestDelegate _, string providerName, string connectionString)
        {
            this._ConnectionString = connectionString;
            this._DbProviderFactory = DbProviderFactories.GetFactory(providerName);
        }

        public async Task Invoke(HttpContext httpContext, IServiceProvider serviceProvider, ISchemaStore schemaStore)
        {
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new ObjectJsonConverter());
            var request = await JsonSerializer.DeserializeAsync<DeleteRequest>(httpContext.Request.Body, jsonOptions);
            var requestValidator = serviceProvider.GetService<IRequestValidator<DeleteRequest>>();
            var valid = requestValidator == null || await requestValidator.Validate(request, httpContext);
            if (valid)
            {
                using var connection = this._DbProviderFactory.CreateConnection();
                connection.ConnectionString = this._ConnectionString;
                await connection.OpenAsync();
                var schema = schemaStore.GetObjectSchema(connection, request.From);
                request.From = schema.Name;
                var validator = new SchemaValidator(schema);
                validator.Validate(request);
                await httpContext.Response.WriteAsync(request.ToSql());
            }
        }
    }
}
