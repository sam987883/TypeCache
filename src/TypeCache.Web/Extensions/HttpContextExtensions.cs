// Copyright (c) 2021 Samuel Abraham

using System.Text.Json;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace TypeCache.Web.Extensions;

public static class HttpContextExtensions
{
	extension(HttpContext @this)
	{
		public async ValueTask<T?> GetJsonRequestAsync<T>(JsonSerializerOptions? options = null)
			=> await JsonSerializer.DeserializeAsync<T>(@this.Request.Body, options, @this.RequestAborted);

		public async ValueTask<string> GetRequestAsync()
		{
			using var reader = new StreamReader(@this.Request.Body);
			return await reader.ReadToEndAsync(@this.RequestAborted);
		}

		public async ValueTask WriteJsonResponseAsync<T>(T response, JsonSerializerOptions? options = null)
		{
			@this.Response.ContentType = Application.Json;
			@this.Response.StatusCode = StatusCodes.Status200OK;
			await JsonSerializer.SerializeAsync(@this.Response.Body, response, options, @this.RequestAborted);
		}

		public async ValueTask WriteResponseAsync(string response)
		{
			@this.Response.ContentType = Text.Plain;
			@this.Response.StatusCode = StatusCodes.Status200OK;
			using var writer = new StreamWriter(@this.Request.Body);
			await writer.WriteAsync(response);
		}
	}
}
