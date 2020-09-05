// Copyright (c) 2020 Samuel Abraham

using Microsoft.AspNetCore.Http;
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
	public class StoredProcedureMiddleware
    {
        private readonly string _ConnectionString;
        private readonly DbProviderFactory _DbProviderFactory;

        public StoredProcedureMiddleware(RequestDelegate _, string providerName, string connectionString)
        {
            this._ConnectionString = connectionString;
            this._DbProviderFactory = DbProviderFactories.GetFactory(providerName);
        }

        public async Task Invoke(HttpContext httpContext, IServiceProvider serviceProvider, ISchemaStore schemaStore)
        {
			string procedure = httpContext.Request.Query[nameof(procedure)].First().Value;
			var request = new StoredProcedureRequest
			{
				Procedure = !procedure.IsBlank() ? procedure : throw new ArgumentException("Query parameter with stored procedure name not found.", nameof(procedure))
			};

			var parameters = httpContext.Request.Query
				.If(query => !query.Key.Equals(nameof(procedure), StringComparison.OrdinalIgnoreCase))
				.To(query => new Parameter
				{
					Name = query.Key,
					Value = query.Value.First().Value
				}).ToList();

			var json = await JsonSerializer.DeserializeAsync<JsonElement>(httpContext.Request.Body);
            if (json.ValueKind == JsonValueKind.Object)
			{
				using var enumerator = json.EnumerateObject();
				while (enumerator.MoveNext())
				{
					var property = enumerator.Current;
					parameters.Add(new Parameter
					{
						Name = property.Name,
						Value = property.Value.ValueKind switch
						{
							JsonValueKind.Undefined => throw new NotImplementedException(),
							JsonValueKind.Object => property.Value.ToString(),
							JsonValueKind.Array => property.Value.ToString(),
							JsonValueKind.String => property.Value.GetString(),
							JsonValueKind.Number => property.Value.TryGetInt64(out var value) ? value : property.Value.GetDecimal(),
							JsonValueKind.True => true,
							JsonValueKind.False => false,
							JsonValueKind.Null => DBNull.Value,
							_ => throw new ArgumentException($"[JsonValueKind.{property.Value.ValueKind.Name()}] stored procedure parameter value: {property.Value}", property.Name)
						}
					});
				}
			}
			else if (json.ValueKind != JsonValueKind.Undefined && json.ValueKind != JsonValueKind.Null)
				throw new ArgumentException("Request body can only contain stored procedure parameters as an object.", "execute");

			request.Parameters = parameters.ToArray();

			var requestValidator = serviceProvider.GetService<IRequestValidator<StoredProcedureRequest>>();
			var valid = requestValidator == null || await requestValidator.Validate(request, httpContext);
            if (valid)
            {
                using var connection = this._DbProviderFactory.CreateConnection();
                connection.ConnectionString = this._ConnectionString;
                await connection.OpenAsync();
				var schema = schemaStore.GetObjectSchema(connection, request.Procedure);
				request.Procedure = schema.Name;
				var validator = new SchemaValidator(schema);
				validator.Validate(request);
				var output = connection.Call(request);
				var response = new StoredProcedureResponse
				{
					Parameters = request.Parameters,
					Output = output
				};
                await JsonSerializer.SerializeAsync(httpContext.Response.Body, output);
            }
		}
	}
}
