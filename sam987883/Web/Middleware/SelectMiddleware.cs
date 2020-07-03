// Copyright (c) 2020 Samuel Abraham

using Microsoft.AspNetCore.Http;
using sam987883.Database;
using sam987883.Database.Commands;
using sam987883.Database.Extensions;
using System.Data.Common;
using System.Text.Json;
using System.Threading.Tasks;

namespace sam987883.Web.Middleware
{
	public class SelectMiddleware
    {
        private readonly string _ConnectionString;
        private readonly DbProviderFactory _DbProviderFactory;

        public SelectMiddleware(RequestDelegate _, string providerName, string connectionString)
        {
            this._ConnectionString = connectionString;
            this._DbProviderFactory = DbProviderFactories.GetFactory(providerName);
        }

        public async Task Invoke(HttpContext httpContext, ISchemaStore schemaStore, IRequestValidator<Select> requestValidator)
        {
            var request = await JsonSerializer.DeserializeAsync<Select>(httpContext.Request.Body);
            var valid = requestValidator == null || await requestValidator.Validate(request, httpContext);
            if (valid)
            {
                using (var connection = this._DbProviderFactory.CreateConnection())
                {
                    connection.ConnectionString = this._ConnectionString;

                    await connection.OpenAsync();
                    var tableSchema = schemaStore.GetTableSchema(connection, request.From);
                    var validator = new SchemaValidator(tableSchema);
                    validator.Validate(request);
                    connection.Select(request);
                }

                await JsonSerializer.SerializeAsync(httpContext.Response.Body, request.Output);
            }
        }
    }
}
