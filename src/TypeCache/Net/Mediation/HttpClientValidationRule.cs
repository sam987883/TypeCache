// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Net.Mediation;

internal sealed class HttpClientValidationRule
	: IValidationRule<HttpClientRequest>
{
	public Task Validate(HttpClientRequest request, CancellationToken token) => Task.Run(() =>
	{
		request.AssertNotNull();
		request.Message.AssertNotNull();
		request.Message.RequestUri.AssertNotNull();

		request.Message.RequestUri.IsAbsoluteUri.AssertTrue();
	}, token);
}
