// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Net.Mediation;

internal sealed class HttpClientRule : IRule<HttpClientRequest, HttpResponseMessage>
{
	private readonly IHttpClientFactory _Factory;

	public HttpClientRule(IHttpClientFactory factory)
	{
		this._Factory = factory;
	}

	public async Task<HttpResponseMessage> MapAsync(HttpClientRequest request, CancellationToken token = default)
	{
		var httpClient = request.HttpClientName.IsNotBlank() ? this._Factory.CreateClient(request.HttpClientName) : this._Factory.CreateClient();
		using var httpResponse = await httpClient.SendAsync(request.Message, HttpCompletionOption.ResponseContentRead, token);
		await httpResponse.Content.LoadIntoBufferAsync();
		return httpResponse;
	}
}
