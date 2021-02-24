// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;

namespace TypeCache.Reflection
{
	public readonly struct ReturnParameter
	{
		public IImmutableList<Attribute> Attributes { get; init; }

		public bool IsTask { get; init; }

		public bool IsValueTask { get; init; }

		public bool IsVoid { get; init; }

		public TypeMember Type { get; init; }
	}
}
