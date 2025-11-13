// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.Logging;
using TypeCache.Mediation;

namespace TypeCache.Web.Mediation;

internal sealed class HttpClientRule(HttpClient httpClient, ILogger<IMediator>? logger = null)
	: IRule<HttpClientRequest, HttpResponseMessage>
{
	public async ValueTask<HttpResponseMessage> Send(HttpClientRequest request, CancellationToken token = default)
	{
		logger?.LogInformation("START: {Method} {RequestUri}", request.Message.Method.Method, request.Message.RequestUri);

		try
		{
			var httpResponse = await httpClient.SendAsync(request.Message, HttpCompletionOption.ResponseContentRead, token);
			await httpResponse.Content.LoadIntoBufferAsync();

			var logLevel = (int)httpResponse.StatusCode switch
			{
				>= 500 => LogLevel.Error,
				>= 400 => LogLevel.Warning,
				_ => LogLevel.Information
			};
			logger?.Log(logLevel, "END: {Method} {RequestUri} - {StatusCode}", request.Message.Method.Method, request.Message.RequestUri, httpResponse.StatusCode);

			return httpResponse;
		}
		catch (Exception error)
		{
			logger?.LogError(error, "ERROR: {Method} {RequestUri} - {ErrorMessage}", request.Message.Method.Method, request.Message.RequestUri, error.Message);

			throw;
		}
	}
}
