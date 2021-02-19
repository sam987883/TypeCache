// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading.Tasks;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection
{
	public readonly struct Parameter
	{
		public IImmutableList<Attribute> Attributes { get; init; }

		public object? DefaultValue { get; init; }

		public bool HasDefaultValue { get; init; }

		public bool IsOptional { get; init; }

		public bool IsOut { get; init; }

		public string Name { get; init; }

		public TypeMember Type { get; init; }
	}
}
