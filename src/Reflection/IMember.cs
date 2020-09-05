// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Immutable;

namespace Sam987883.Reflection
{
	public interface IMember
	{
		IImmutableList<RuntimeTypeHandle> ArrayTypeHandles { get; }

		IImmutableList<Attribute> Attributes { get; }

		bool Internal { get; }

		bool IsNullable { get; }

		string Name { get; }

		bool Public { get; }

		NativeType Type { get; }

		RuntimeTypeHandle TypeHandle { get; }
	}
}
