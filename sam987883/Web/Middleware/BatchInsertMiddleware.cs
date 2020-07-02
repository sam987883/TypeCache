// Copyright (c) 2020 Samuel Abraham

using Microsoft.AspNetCore.Http;
using sam987883.Database;
using sam987883.Database.Extensions;
using System.Data.Common;
using System.Text.Json;
using System.Threading.Tasks;

namespace sam987883.Web.Middleware
{
	public class BatchInsertMiddleware
    {
        private readonly string _ConnectionString;
        private readonly DbProviderFactory _DbProviderFactory;

        public BatchInsertMiddleware(RequestDelegate _, string providerName, string connectionString)
        {
            this._ConnectionString = connectionString;
            this._DbProviderFactory = DbProviderFactories.GetFactory(providerName);
        }

        public async Task Invoke(HttpContext httpContext, ISchemaStore schemaStore, IRequestValidator<BatchInsert> requestValidator)
        {
            var request = await JsonSerializer.DeserializeAsync<BatchInsert>(httpContext.Request.Body);
            var valid = requestValidator == null || await requestValidator.Validate(request, httpContext);
            if (valid)
            {
                using (var connection = this._DbProviderFactory.CreateConnection())
                {
                    connection.ConnectionString = this._ConnectionString;

                    await connection.OpenAsync();
                    var tableSchema = schemaStore.GetTableSchema(connection, request.Table);
                    var validator = new SchemaValidator(tableSchema);
                    validator.Validate(request);
                    connection.Insert(request);
                }

                await JsonSerializer.SerializeAsync(httpContext.Response.Body, request.Output);
            }
        }
    }
}
