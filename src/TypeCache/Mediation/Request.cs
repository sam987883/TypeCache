// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public readonly record struct Request<REQUEST>(REQUEST Value, object? ServiceKey)
{
	public Request<REQUEST, RESPONSE> For<RESPONSE>()
		=> new(this.Value, this.ServiceKey);

	public Request<REQUEST> Key(object key)
		=> new(this.Value, key);
}

public readonly record struct Request<REQUEST, RESPONSE>(REQUEST Value, object? ServiceKey);
