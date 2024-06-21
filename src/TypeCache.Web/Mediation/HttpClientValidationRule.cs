﻿// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using TypeCache.Mediation;

namespace TypeCache.Web.Mediation;

internal sealed class HttpClientValidationRule
	: IValidationRule<HttpClientRequest>
{
	public Task Validate(HttpClientRequest request, CancellationToken token) => Task.Run(() =>
	{
		request.ThrowIfNull();
		request.Message.ThrowIfNull();
		request.Message.RequestUri.ThrowIfNull();
		request.Message.RequestUri.IsAbsoluteUri.ThrowIfFalse();
	}, token);
}
