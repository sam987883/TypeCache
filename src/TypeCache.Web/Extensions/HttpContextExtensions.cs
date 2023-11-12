// Copyright (c) 2021 Samuel Abraham

using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace TypeCache.Web.Extensions;

public static class HttpContextExtensions
{
	public static async ValueTask<T?> GetJsonRequestAsync<T>(this HttpContext @this, JsonSerializerOptions? options = null)
		=> await JsonSerializer.DeserializeAsync<T>(@this.Request.Body, options, @this.RequestAborted);

	public static async ValueTask<string> GetRequestAsync(this HttpContext @this)
	{
		using var reader = new StreamReader(@this.Request.Body);
		return await reader.ReadToEndAsync(@this.RequestAborted);
	}

	public static async ValueTask WriteJsonResponseAsync<T>(this HttpContext @this, T response, JsonSerializerOptions? options = null)
	{
		@this.Response.ContentType = Application.Json;
		@this.Response.StatusCode = StatusCodes.Status200OK;
		await JsonSerializer.SerializeAsync(@this.Response.Body, response, options, @this.RequestAborted);
	}

	public static async ValueTask WriteResponseAsync(this HttpContext @this, string response)
	{
		@this.Response.ContentType = Text.Plain;
		@this.Response.StatusCode = StatusCodes.Status200OK;
		using var writer = new StreamWriter(@this.Request.Body);
		await writer.WriteAsync(response);
	}
}
