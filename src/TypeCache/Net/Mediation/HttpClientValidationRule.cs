// Copyright (c) 2021 Samuel Abraham

using TypeCache.Mediation;

namespace TypeCache.Net.Mediation;

internal sealed class HttpClientValidationRule
	: IValidationRule<HttpClientRequest>
{
	public IEnumerable<string> Validate(HttpClientRequest request)
	{
		var validator = new Validator();
		validator.AssertNotNull(request?.Message?.RequestUri);

		if (validator.Success)
			validator.AssertEquals(request!.Message.RequestUri!.IsAbsoluteUri, true);

		return validator.Fails;
	}
}
