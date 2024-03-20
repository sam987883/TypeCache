// Copyright (c) 2021 Samuel Abraham

using GraphQL.DI;
using GraphQL.Types;

namespace TypeCache.GraphQL.Types;

internal sealed class ConfigureSchema(Action<ISchema, IServiceProvider> configure) : IConfigureSchema
{
	public void Configure(ISchema schema, IServiceProvider serviceProvider)
		=> configure?.Invoke(schema, serviceProvider);
}
