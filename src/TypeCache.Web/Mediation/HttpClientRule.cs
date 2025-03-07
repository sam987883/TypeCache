﻿// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.Logging;
using TypeCache.Mediation;

namespace TypeCache.Web.Mediation;

internal sealed class HttpClientRule(HttpClient httpClient, ILogger<IMediator>? logger = null)
	: IRule<HttpClientRequest, HttpResponseMessage>
{
	public async Task<HttpResponseMessage> Map(HttpClientRequest request, CancellationToken token = default)
	{
		logger?.LogInformation(Invariant($"{{{nameof(request.Message.Method)}}} {{{nameof(request.Message.RequestUri)}}}"),
			[request.Message.Method.Method, request.Message.RequestUri]);

		try
		{
			var httpResponse = await httpClient.SendAsync(request.Message, HttpCompletionOption.ResponseContentRead, token);
			await httpResponse.Content.LoadIntoBufferAsync();

			var logLevel = httpResponse.IsSuccessStatusCode ? LogLevel.Information : LogLevel.Error;
			logger?.Log(logLevel, Invariant($"{{{nameof(request.Message.Method)}}} {{{nameof(request.Message.RequestUri)}}} - {{{nameof(httpResponse.StatusCode)}}}"),
				request.Message.Method.Method, request.Message.RequestUri, httpResponse.StatusCode);

			return httpResponse;
		}
		catch (Exception error)
		{
			logger?.LogError(error, Invariant($"{{{nameof(request.Message.Method)}}} {{{nameof(request.Message.RequestUri)}}} - {{ErrorMessage}}"),
				request.Message.Method.Method, request.Message.RequestUri, error.Message);

			throw;
		}
	}
}
