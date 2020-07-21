// Copyright (c) 2020 Samuel Abraham

using Microsoft.AspNetCore.Http;
using sam987883.Database;
using sam987883.Database.Extensions;
using sam987883.Database.Requests;
using sam987883.Dependencies;
using sam987883.Extensions;
using System;
using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace sam987883.Web.Middleware
{
	public class SelectSQLMiddleware
    {
        private readonly string _ConnectionString;
        private readonly DbProviderFactory _DbProviderFactory;

        public SelectSQLMiddleware(RequestDelegate _, string providerName, string connectionString)
        {
            this._ConnectionString = connectionString;
            this._DbProviderFactory = DbProviderFactories.GetFactory(providerName);
        }

        public async Task Invoke(HttpContext httpContext, IServiceProvider serviceProvider, ISchemaStore schemaStore)
        {
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new ParameterJsonConverter());
            jsonOptions.Converters.Add(new ExpressionJsonConverter());
            var request = await JsonSerializer.DeserializeAsync<SelectRequest>(httpContext.Request.Body, jsonOptions);
            var requestValidator = serviceProvider.GetService<IRequestValidator<SelectRequest>>();
            var valid = requestValidator == null || await requestValidator.Validate(request, httpContext);
            if (valid)
            {
                using var connection = this._DbProviderFactory.CreateConnection();
                connection.ConnectionString = this._ConnectionString;
                await connection.OpenAsync();
                var objectSchema = schemaStore.GetObjectSchema(connection, request.From);
                request.From = objectSchema.Name;
                var validator = new SchemaValidator(objectSchema);
                validator.Validate(request);
                await httpContext.Response.WriteAsync(request.ToSql());
            }
        }
    }
}
