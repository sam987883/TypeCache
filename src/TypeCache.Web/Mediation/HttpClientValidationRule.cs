// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Web.Mediation;

internal sealed class HttpClientValidationRule
	: IValidation<HttpRequestMessage>
{
	public void Validate(HttpRequestMessage request)
	{
		request.ThrowIfNull();
		request.RequestUri.ThrowIfNull();
		request.RequestUri.IsAbsoluteUri.ThrowIfFalse();
	}
}
