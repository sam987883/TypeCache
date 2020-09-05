// Copyright (c) 2020 Samuel Abraham

using Microsoft.AspNetCore.Http;
using Sam987883.Common.Converters;
using Sam987883.Common.Extensions;
using Sam987883.Database;
using Sam987883.Database.Extensions;
using Sam987883.Database.Models;
using System;
using System.Data.Common;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sam987883.Web.Middleware
{
	public class SQLMiddleware
    {
        private readonly string _ConnectionString;
        private readonly DbProviderFactory _DbProviderFactory;

        public SQLMiddleware(RequestDelegate _, string providerName, string connectionString)
        {
            this._ConnectionString = connectionString;
            this._DbProviderFactory = DbProviderFactories.GetFactory(providerName);
        }

        public async Task Invoke(HttpContext httpContext)
        {
            using var reader = new StreamReader(httpContext.Request.Body);
            var request = new SQLRequest
            {
                Parameters = httpContext.Request.Query
                    .To(query => new Parameter
                    {
                        Name = query.Key,
                        Value = query.Value.First().Value
                    }).ToArray(httpContext.Request.Query.Count),
                SQL = reader.ReadToEnd()
            };
            using var connection = this._DbProviderFactory.CreateConnection();
            connection.ConnectionString = this._ConnectionString;
            await connection.OpenAsync();
            var output = connection.Run(request);
            await JsonSerializer.SerializeAsync(httpContext.Response.Body, output);
        }
    }
}
