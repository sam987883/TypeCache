// Copyright (c) 2020 Samuel Abraham

using Microsoft.AspNetCore.Http;
using Sam987883.Common.Converters;
using Sam987883.Common.Extensions;
using Sam987883.Database;
using Sam987883.Database.Extensions;
using Sam987883.Database.Models;
using System;
using System.Data.Common;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sam987883.Web.Middleware
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

        public async Task Invoke(HttpContext httpContext, IServiceProvider serviceProvider, ISchemaStore schemaStore)
        {
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new ObjectJsonConverter());
            var request = await JsonSerializer.DeserializeAsync<SelectRequest>(httpContext.Request.Body, jsonOptions);
            var requestValidator = serviceProvider.GetService<IRequestValidator<SelectRequest>>();
            var valid = requestValidator == null || await requestValidator.Validate(request, httpContext);
            if (valid)
            {
                using var connection = this._DbProviderFactory.CreateConnection();
                connection.ConnectionString = this._ConnectionString;
                await connection.OpenAsync();
                var output = connection.Select(request, schemaStore);
                await JsonSerializer.SerializeAsync(httpContext.Response.Body, output);
            }
        }
    }
}
