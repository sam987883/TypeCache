// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.Logging;
using TypeCache.Attributes;
using TypeCache.Mediation;

namespace TypeCache.Web.Mediation;

[Scoped]
internal sealed class HttpClientRule(HttpClient httpClient, ILogger<IMediator>? logger = null)
	: IRule<HttpRequestMessage, ValueTask<HttpResponseMessage>>
{
	public async ValueTask<HttpResponseMessage> Send(HttpRequestMessage request, CancellationToken token = default)
	{
		logger?.LogInformation("START: {Method} {RequestUri}", request.Method.Method, request.RequestUri);

		try
		{
			var httpResponse = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, token);
			await httpResponse.Content.LoadIntoBufferAsync();

			var logLevel = (int)httpResponse.StatusCode switch
			{
				>= 500 => LogLevel.Error,
				>= 400 => LogLevel.Warning,
				_ => LogLevel.Information
			};
			logger?.Log(logLevel, "END: {Method} {RequestUri} - {StatusCode}", request.Method.Method, request.RequestUri, httpResponse.StatusCode);

			return httpResponse;
		}
		catch (Exception error)
		{
			logger?.LogError(error, "ERROR: {Method} {RequestUri} - {ErrorMessage}", request.Method.Method, request.RequestUri, error.Message);

			throw;
		}
	}
}
