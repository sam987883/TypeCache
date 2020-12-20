// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Http;
using System.Data.Common;
using System.Text.Json;
using System.Threading.Tasks;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Web.Middleware
{
	public class SchemaMiddleware
    {
        private readonly string _ConnectionString;
        private readonly DbProviderFactory _DbProviderFactory;

        public SchemaMiddleware(RequestDelegate _, string providerName, string connectionString)
        {
            this._ConnectionString = connectionString;
            this._DbProviderFactory = DbProviderFactories.GetFactory(providerName);
        }

        public async Task Invoke(HttpContext httpContext, ISchemaStore schemaStore)
        {
            var name = httpContext.Request.Query["object"].First();
            if (!name.IsBlank())
            {
                using var connection = this._DbProviderFactory.CreateConnection();
                connection.ConnectionString = this._ConnectionString;
                await connection.OpenAsync();
                var schema = schemaStore.GetObjectSchema(connection, name);
                await JsonSerializer.SerializeAsync(httpContext.Response.Body, schema);
            }
        }
    }
}
