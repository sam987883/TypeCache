// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL.DI;
using GraphQL.Types;
using TypeCache.Extensions;

namespace TypeCache.GraphQL.Types;

internal class ConfigureSchema : IConfigureSchema
{
	private readonly Action<ISchema, IServiceProvider> _Configure;

	public ConfigureSchema(Action<ISchema, IServiceProvider> configure)
	{
		configure.AssertNotNull();

		this._Configure = configure;
	}

	public void Configure(ISchema schema, IServiceProvider serviceProvider)
		=> this._Configure(schema, serviceProvider);
}
