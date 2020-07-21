// Copyright (c) 2020 Samuel Abraham

using Microsoft.AspNetCore.Http;
using sam987883.Database;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace sam987883.Web.Middleware
{
	public class GraphMiddleware
    {
        private readonly string _ConnectionString;
        private readonly DbProviderFactory _DbProviderFactory;

        public GraphMiddleware(RequestDelegate _, string providerName, string connectionString)
        {
            this._ConnectionString = connectionString;
            this._DbProviderFactory = DbProviderFactories.GetFactory(providerName);
        }

        public async Task Invoke(HttpContext httpContext, IServiceProvider serviceProvider, ISchemaStore schemaStore)
        {
            throw new NotImplementedException("Coming soon.");
        }
    }
}
