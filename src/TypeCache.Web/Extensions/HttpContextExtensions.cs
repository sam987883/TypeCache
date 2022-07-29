﻿// Copyright (c) 2021 Samuel Abraham

using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace TypeCache.Web.Extensions;

public static class HttpContextExtensions
{
	public static async ValueTask<T?> GetJsonRequestAsync<T>(this HttpContext @this, JsonSerializerOptions? options = null, CancellationToken token = default)
		=> await JsonSerializer.DeserializeAsync<T>(@this.Request.Body, options, token);

	public static async ValueTask<string> GetRequestAsync(this HttpContext @this)
	{
		using var reader = new StreamReader(@this.Request.Body);
		return await reader.ReadToEndAsync();
	}

	public static async ValueTask WriteJsonResponseAsync<T>(this HttpContext @this, T response, JsonSerializerOptions? options = null, CancellationToken token = default)
	{
		@this.Response.ContentType = Application.Json;
		@this.Response.StatusCode = (int)HttpStatusCode.OK;
		await JsonSerializer.SerializeAsync(@this.Response.Body, response, options, token);
	}

	public static async ValueTask WriteResponseAsync(this HttpContext @this, string response)
	{
		@this.Response.ContentType = Text.Plain;
		@this.Response.StatusCode = (int)HttpStatusCode.OK;
		using var writer = new StreamWriter(@this.Request.Body);
		await writer.WriteAsync(response);
	}
}
