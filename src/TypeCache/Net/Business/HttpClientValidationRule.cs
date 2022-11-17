// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Net.Http;
using TypeCache.Business;

namespace TypeCache.Net.Business;

internal sealed class HttpClientValidationRule
	: IValidationRule<HttpRequestMessage>
	, IValidationRule<(HttpRequestMessage HttpRequest, string HttpClientName)>
{
	public IEnumerable<string> Validate(HttpRequestMessage request)
	{
		var validator = new Validator();
		validator.AssertNotNull(request?.RequestUri);

		if (validator.Success)
			validator.AssertEquals(request!.RequestUri!.IsAbsoluteUri, true);

		return validator.Fails;
	}

	public IEnumerable<string> Validate((HttpRequestMessage HttpRequest, string HttpClientName) request)
	{
		var validator = new Validator();
		validator.AssertNotNull(request.HttpRequest?.RequestUri);

		if (validator.Success)
			validator.AssertEquals(request.HttpRequest!.RequestUri!.IsAbsoluteUri, true);

		return validator.Fails;
	}
}
