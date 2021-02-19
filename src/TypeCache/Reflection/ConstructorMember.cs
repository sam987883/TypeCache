// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;

namespace TypeCache.Reflection
{
	public readonly struct ConstructorMember
	{
		public IImmutableList<Attribute> Attributes { get; init; }

		public RuntimeMethodHandle Handle { get; init; }

		public StaticInvokeType Invoke { get; init; }

		public bool IsInternal { get; init; }

		public bool IsPublic { get; init; }

		public Delegate Method { get; init; }

		public IImmutableList<Parameter> Parameters { get; init; }

		public TypeMember Type { get; init; }
	}
}
