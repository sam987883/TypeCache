// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Data
{
	public sealed record ParameterSchema()
	{
		public int Id { get; init; }

		public string Name { get; init; } = string.Empty;

		public bool Output { get; init; }

		public bool Return { get; init; }

		public SqlDbType Type { get; init; }
	}
}
