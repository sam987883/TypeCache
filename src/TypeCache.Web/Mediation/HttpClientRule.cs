// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.Logging;
using TypeCache.Mediation;

namespace TypeCache.Web.Mediation;

internal sealed class HttpClientRule
	: IRule<HttpClientRequest, HttpResponseMessage>
{
	private readonly HttpClient _HttpClient;
	private readonly ILogger<IMediator>? _Logger;

	public HttpClientRule(HttpClient httpClient, ILogger<IMediator>? logger = null)
	{
		this._HttpClient = httpClient;
		this._Logger = logger;
	}

	public async Task<HttpResponseMessage> Map(HttpClientRequest request, CancellationToken token = default)
	{
		this._Logger?.LogInformation(Invariant($"{{{nameof(request.Message.Method)}}} {{{nameof(request.Message.RequestUri)}}}"),
			request.Message.Method.Method, request.Message.RequestUri);

		try
		{
			var httpResponse = await this._HttpClient.SendAsync(request.Message, HttpCompletionOption.ResponseContentRead, token);
			await httpResponse.Content.LoadIntoBufferAsync();

			var logLevel = httpResponse.IsSuccessStatusCode ? LogLevel.Information : LogLevel.Error;
			this._Logger?.Log(logLevel, Invariant($"{{{nameof(request.Message.Method)}}} {{{nameof(request.Message.RequestUri)}}} - {{{nameof(httpResponse.StatusCode)}}}"),
				request.Message.Method.Method, request.Message.RequestUri, httpResponse.StatusCode);

			return httpResponse;
		}
		catch (Exception error)
		{
			this._Logger?.LogError(error, Invariant($"{{{nameof(request.Message.Method)}}} {{{nameof(request.Message.RequestUri)}}} - {{ErrorMessage}}"),
				request.Message.Method.Method, request.Message.RequestUri, error.Message);

			throw;
		}
	}
}
