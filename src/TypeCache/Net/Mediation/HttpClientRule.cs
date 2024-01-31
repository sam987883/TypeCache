// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Net.Mediation;

internal sealed class HttpClientRule(IHttpClientFactory factory)
	: IRule<HttpClientRequest, HttpResponseMessage>
{
	public async Task<HttpResponseMessage> Map(HttpClientRequest request, CancellationToken token = default)
	{
		var httpClient = request.HttpClientName.IsNotBlank() ? factory.CreateClient(request.HttpClientName) : factory.CreateClient();
		using var httpResponse = await httpClient.SendAsync(request.Message, HttpCompletionOption.ResponseContentRead, token);
		await httpResponse.Content.LoadIntoBufferAsync();
		return httpResponse;
	}
}
