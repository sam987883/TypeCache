// Copyright (c) 2021 Samuel Abraham

using TypeCache.Business;
using TypeCache.Extensions;

namespace TypeCache.Net.Business;

internal sealed class HttpClientRule : IRule<HttpRequestMessage, HttpResponseMessage>, IRule<(HttpRequestMessage HttpRequest, string HttpClientName), HttpResponseMessage>
{
	private readonly IHttpClientFactory _Factory;

	public HttpClientRule(IHttpClientFactory factory)
	{
		this._Factory = factory;
	}

	public async ValueTask<HttpResponseMessage> ApplyAsync(HttpRequestMessage request, CancellationToken token = default)
	{
		request.AssertNotNull();

		var httpClient = this._Factory.CreateClient();
		using var httpResponse = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, token);
		await httpResponse.Content.LoadIntoBufferAsync();
		return httpResponse;
	}

	public async ValueTask<HttpResponseMessage> ApplyAsync((HttpRequestMessage HttpRequest, string HttpClientName) request, CancellationToken token = default)
	{
		request.AssertNotNull();
		request.HttpClientName.AssertNotBlank();

		var httpClient = this._Factory.CreateClient(request.HttpClientName);
		using var httpResponse = await httpClient.SendAsync(request.HttpRequest, HttpCompletionOption.ResponseContentRead, token);
		await httpResponse.Content.LoadIntoBufferAsync();
		return httpResponse;
	}
}
