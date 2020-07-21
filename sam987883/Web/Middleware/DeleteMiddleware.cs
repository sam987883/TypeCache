// Copyright (c) 2020 Samuel Abraham

using Microsoft.AspNetCore.Http;
using sam987883.Database;
using sam987883.Database.Requests;
using sam987883.Database.Extensions;
using sam987883.Dependencies;
using System.Data.Common;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace sam987883.Web.Middleware
{
	public class DeleteMiddleware
    {
        private readonly string _ConnectionString;
        private readonly DbProviderFactory _DbProviderFactory;

        public DeleteMiddleware(RequestDelegate _, string providerName, string connectionString)
        {
            this._ConnectionString = connectionString;
            this._DbProviderFactory = DbProviderFactories.GetFactory(providerName);
        }

        public async Task Invoke(HttpContext httpContext, IServiceProvider serviceProvider, ISchemaStore schemaStore)
        {
            var request = await JsonSerializer.DeserializeAsync<DeleteRequest>(httpContext.Request.Body);
            var requestValidator = serviceProvider.GetService<IRequestValidator<DeleteRequest>>();
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
                var output = connection.Delete(request);
                await JsonSerializer.SerializeAsync(httpContext.Response.Body, output);
            }
        }
    }
}
