// Copyright (c) 2021 Samuel Abraham

using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Web.Middleware
{
	public class StoredProcedureMiddleware : DataMiddleware
	{
		public StoredProcedureMiddleware(RequestDelegate _, ISqlApi sqlApi, IMediator mediator)
			: base(sqlApi, mediator)
		{
		}

		public async Task Invoke(HttpContext httpContext)
		{
			string procedure = httpContext.Request.Query[nameof(procedure)].First()!;
			var parameters = httpContext.Request.Query
				.If(query => !query.Key.Is(nameof(procedure)))
				.To(query => new Parameter(query.Key, query.Value.First()))
				.ToList();

			var json = await JsonSerializer.DeserializeAsync<JsonElement>(httpContext.Request.Body);
			if (json.ValueKind == JsonValueKind.Object)
			{
				using var enumerator = json.EnumerateObject();
				while (enumerator.MoveNext())
				{
					var property = enumerator.Current;
					parameters.Add(new Parameter(property.Name, property.Value.ValueKind switch
					{
						JsonValueKind.Undefined => throw new NotImplementedException(),
						JsonValueKind.Object => property.Value.ToString(),
						JsonValueKind.Array => property.Value.ToString(),
						JsonValueKind.String => property.Value.GetString(),
						JsonValueKind.Number => property.Value.TryGetInt64(out var value) ? value : property.Value.GetDecimal(),
						JsonValueKind.True => true,
						JsonValueKind.False => false,
						JsonValueKind.Null => DBNull.Value,
						_ => DBNull.Value
					}));
				}
			}

			var request = new StoredProcedureRequest
			{
				Procedure = procedure,
				Parameters = parameters.ToArray()
			};

			await this.HandleRequest<StoredProcedureRequest, RowSet[]>(request, httpContext);
		}
	}
}
