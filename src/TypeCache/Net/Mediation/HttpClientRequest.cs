// Copyright (c) 2021 Samuel Abraham

using TypeCache.Mediation;

namespace TypeCache.Net.Mediation;

public sealed record class HttpClientRequest(HttpRequestMessage Message) : IRequest<HttpResponseMessage>
{
	public string HttpClientName { get; set; } = string.Empty;
}
