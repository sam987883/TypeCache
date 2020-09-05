// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Immutable;

namespace Sam987883.Reflection
{
	public interface IParameter
	{
		IImmutableList<RuntimeTypeHandle> ArrayTypeHandles { get; }

		IImmutableList<Attribute> Attributes { get; }

		object? DefaultValue { get; }

		bool HasDefaultValue { get; }

		bool IsNullable { get; }

		string Name { get; }

		bool Optional { get; }

		bool Out { get; }

		NativeType Type { get; }

		RuntimeTypeHandle TypeHandle { get; }

		bool Supports(Type type);
	}
}
