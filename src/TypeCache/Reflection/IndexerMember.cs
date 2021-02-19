// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;

namespace TypeCache.Reflection
{
	public readonly struct IndexerMember
	{
		public IImmutableList<Attribute> Attributes { get; init; }

		public bool IsInternal { get; init; }

		public bool IsPublic { get; init; }

		public TypeMember Type { get; init; }

		public MethodMember? GetMethod { get; init; }

		public MethodMember? SetMethod { get; init; }
	}
}
