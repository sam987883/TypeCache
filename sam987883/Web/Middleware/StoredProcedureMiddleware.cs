// Copyright (c) 2020 Samuel Abraham

using Microsoft.AspNetCore.Http;
using sam987883.Database;
using sam987883.Database.Extensions;
using sam987883.Database.Requests;
using sam987883.Database.Responses;
using sam987883.Dependencies;
using sam987883.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.Json;
using System.Threading.Tasks;

namespace sam987883.Web.Middleware
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

			request.Parameters = new Dictionary<string, object?>(httpContext.Request.Query.Count + 9, StringComparer.OrdinalIgnoreCase);
			httpContext.Request.Query
				.If(_ => !_.Key.Equals(nameof(procedure), StringComparison.OrdinalIgnoreCase))
				.Do(query => request.Parameters.Add(query.Key, query.Value.First().Value));

			var json = await JsonSerializer.DeserializeAsync<JsonElement>(httpContext.Request.Body);
            if (json.ValueKind == JsonValueKind.Object)
			{
				using var enumerator = json.EnumerateObject();
				while (enumerator.MoveNext())
				{
					var property = enumerator.Current;
					switch (property.Value.ValueKind)
					{
						case JsonValueKind.Array:
						case JsonValueKind.Object:
							request.Parameters.Add(property.Name, property.Value.ToString());
							break;
						case JsonValueKind.String:
							request.Parameters.Add(property.Name, property.Value.GetString());
							break;
						case JsonValueKind.Number:
							request.Parameters.Add(property.Name, property.Value.TryGetInt64(out var value) ? value : property.Value.GetDecimal());
							break;
						case JsonValueKind.True:
							request.Parameters.Add(property.Name, true);
							break;
						case JsonValueKind.False:
							request.Parameters.Add(property.Name, false);
							break;
						case JsonValueKind.Null:
							request.Parameters.Add(property.Name, null);
							break;
						case JsonValueKind.Undefined:
							throw new ArgumentException($"[JsonValueKind.{nameof(JsonValueKind.Undefined)}] stored procedure parameter value: {property.Value}", property.Name);
					}
				}
			}
			else if (json.ValueKind != JsonValueKind.Undefined && json.ValueKind != JsonValueKind.Null)
				throw new ArgumentException("Request body can only contain stored procedure parameters as an object.", "execute");

			var requestValidator = serviceProvider.GetService<IRequestValidator<StoredProcedureRequest>>();
			var valid = requestValidator == null || await requestValidator.Validate(request, httpContext);
            if (valid)
            {
                using var connection = this._DbProviderFactory.CreateConnection();
                connection.ConnectionString = this._ConnectionString;
                await connection.OpenAsync();
				var objectSchema = schemaStore.GetObjectSchema(connection, request.Procedure);
				request.Procedure = objectSchema.Name;
				var validator = new SchemaValidator(objectSchema);
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
