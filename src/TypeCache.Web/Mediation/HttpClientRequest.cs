// Copyright (c) 2021 Samuel Abraham

using TypeCache.Mediation;

namespace TypeCache.Web.Mediation;

public sealed class HttpClientRequest : IRequest<HttpResponseMessage>
{
	public required HttpRequestMessage Message { get; set; }
}
