// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Http;
using System.Data.Common;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Web.Middleware
{
	public class SqlMiddleware
    {
        private readonly string _ConnectionString;
        private readonly DbProviderFactory _DbProviderFactory;

        public SqlMiddleware(RequestDelegate _, string providerName, string connectionString)
        {
            this._ConnectionString = connectionString;
            this._DbProviderFactory = DbProviderFactories.GetFactory(providerName);
        }

        public async Task Invoke(HttpContext httpContext)
        {
            using var reader = new StreamReader(httpContext.Request.Body);
            var request = new SqlRequest
            {
                Parameters = httpContext.Request.Query
                    .To(query => new Parameter
                    {
                        Name = query.Key,
                        Value = query.Value.First()
                    }).ToArrayOf(httpContext.Request.Query.Count),
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
