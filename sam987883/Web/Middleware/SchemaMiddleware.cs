// Copyright (c) 2020 Samuel Abraham

using Microsoft.AspNetCore.Http;
using sam987883.Database;
using System.Data.Common;
using System.Text.Json;
using System.Threading.Tasks;
using sam987883.Extensions;

namespace sam987883.Web.Middleware
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
            var name = httpContext.Request.Query["object"].First().Value;
            if (!name.IsBlank())
            {
                using var connection = this._DbProviderFactory.CreateConnection();
                connection.ConnectionString = this._ConnectionString;
                await connection.OpenAsync();
                var objectSchema = schemaStore.GetObjectSchema(connection, name);
                await JsonSerializer.SerializeAsync(httpContext.Response.Body, objectSchema);
            }
        }
    }
}
