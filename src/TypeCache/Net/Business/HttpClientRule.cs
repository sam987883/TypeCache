// Copyright (c) 2021 Samuel Abraham

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Extensions;

namespace TypeCache.Net.Business;

internal class HttpClientRule : IRule<HttpRequestMessage, HttpResponseMessage>, IRule<(HttpRequestMessage HttpRequest, string HttpClientName), HttpResponseMessage>
{
	private readonly IHttpClientFactory _Factory;

	public HttpClientRule(IHttpClientFactory factory)
	{
		this._Factory = factory;
	}

	public async ValueTask<HttpResponseMessage> ApplyAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
	{
		request.AssertNotNull();

		using var httpClient = this._Factory.CreateClient();
		using var httpResponse = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
		await httpResponse.Content.LoadIntoBufferAsync();
		return httpResponse;
	}

	public async ValueTask<HttpResponseMessage> ApplyAsync((HttpRequestMessage HttpRequest, string HttpClientName) request, CancellationToken cancellationToken = default)
	{
		request.AssertNotNull();
		request.HttpClientName.AssertNotBlank();

		using var httpClient = this._Factory.CreateClient(request.HttpClientName);
		using var httpResponse = await httpClient.SendAsync(request.HttpRequest, HttpCompletionOption.ResponseContentRead, cancellationToken);
		await httpResponse.Content.LoadIntoBufferAsync();
		return httpResponse;
	}
}
