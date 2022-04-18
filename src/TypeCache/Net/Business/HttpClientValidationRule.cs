// Copyright (c) 2021 Samuel Abraham

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Extensions;

namespace TypeCache.Net.Business;

internal class HttpClientValidationRule : IValidationRule<HttpRequestMessage>, IValidationRule<(HttpRequestMessage HttpRequest, string HttpClientName)>
{
	public async ValueTask ValidateAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
	{
		request.RequestUri.AssertNotNull();
		request.RequestUri!.IsAbsoluteUri.Assert(true);
		await ValueTask.CompletedTask;
	}

	public async ValueTask ValidateAsync((HttpRequestMessage HttpRequest, string HttpClientName) request, CancellationToken cancellationToken = default)
	{
		request.HttpRequest.RequestUri.AssertNotNull();
		request.HttpRequest.RequestUri!.IsAbsoluteUri.Assert(false);
		await ValueTask.CompletedTask;
	}
}
