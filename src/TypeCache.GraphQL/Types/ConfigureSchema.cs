// Copyright (c) 2021 Samuel Abraham

using GraphQL.DI;
using GraphQL.Types;

namespace TypeCache.GraphQL.Types;

internal sealed class ConfigureSchema : IConfigureSchema
{
	private readonly Action<ISchema, IServiceProvider> _Configure;

	public ConfigureSchema(Action<ISchema, IServiceProvider> configure)
	{
		this._Configure = configure;
	}

	public void Configure(ISchema schema, IServiceProvider serviceProvider)
		=> this._Configure?.Invoke(schema, serviceProvider);
}
