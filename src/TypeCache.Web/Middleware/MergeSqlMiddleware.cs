// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Converters;
using TypeCache.Extensions;
using TypeCache.Data;
using TypeCache.Reflection.Converters;
using System;
using System.Data.Common;
using System.Text.Json;
using System.Threading.Tasks;
using TypeCache.Data.Schema;

namespace TypeCache.Web.Middleware
{
	public class MergeSqlMiddleware
    {
        private readonly string _ConnectionString;
        private readonly DbProviderFactory _DbProviderFactory;

        public MergeSqlMiddleware(RequestDelegate _, string providerName, string connectionString)
        {
            this._ConnectionString = connectionString;
            this._DbProviderFactory = DbProviderFactories.GetFactory(providerName);
        }

        public async Task Invoke(HttpContext httpContext, IServiceProvider serviceProvider, ISchemaStore schemaStore, PropertyJsonConverter<BatchRequest> jsonConverter)
        {
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new ObjectJsonConverter());
            var request = await JsonSerializer.DeserializeAsync<BatchRequest>(httpContext.Request.Body, jsonOptions);
            var requestValidator = serviceProvider.GetService<IRequestValidator<BatchRequest>>();
            var valid = requestValidator == null || await requestValidator.Validate(request, httpContext);
            if (valid)
            {
                using var connection = this._DbProviderFactory.CreateConnection();
                connection.ConnectionString = this._ConnectionString;
                await connection.OpenAsync();
                var schema = schemaStore.GetObjectSchema(connection, request.Table);
                request.Table = schema.Name;
                request.On ??= schema.Columns.If(column => column.PrimaryKey).To(column => column.Name).ToList().ToArray();
                var validator = new SchemaValidator(schema);
                validator.Validate(request);
                await httpContext.Response.WriteAsync(request.ToSql());
            }
        }
    }
}
