// Copyright (c) 2021 Samuel Abraham

using TypeCache.Mediation;

namespace TypeCache.Net.Mediation;

public sealed class HttpClientRequest : IRequest<HttpResponseMessage>
{
	public string HttpClientName { get; set; } = string.Empty;

	public required HttpRequestMessage Message { get; set; }
}
