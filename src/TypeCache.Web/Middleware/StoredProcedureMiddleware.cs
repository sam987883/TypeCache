// Copyright (c) 2021 Samuel Abraham

using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Requests;
using TypeCache.Data.Responses;
using TypeCache.Extensions;

namespace TypeCache.Web.Middleware
{
	public class StoredProcedureMiddleware : DataMiddleware
	{
		public StoredProcedureMiddleware(RequestDelegate _, IMediator mediator)
			: base(mediator)
		{
		}

		public async Task Invoke(HttpContext httpContext)
		{
			string procedure = httpContext.Request.Query[nameof(procedure)].First()!;
			var request = new StoredProcedureRequest(procedure);
			httpContext.Request.Query
				.If(query => !query.Key.Is(nameof(procedure)))
				.Do(query => request.Parameters.Add(query.Key, query.Value.First()));

			var json = await JsonSerializer.DeserializeAsync<JsonElement>(httpContext.Request.Body);
			if (json.ValueKind == JsonValueKind.Object)
			{
				using var enumerator = json.EnumerateObject();
				while (enumerator.MoveNext())
				{
					var property = enumerator.Current;
					request.Parameters.Add(property.Name, property.Value.GetValue());
				}
			}

			await this.HandleRequest<StoredProcedureRequest, StoredProcedureResponse>(request, httpContext);
		}
	}
}
