// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;

namespace TypeCache.Reflection
{
	public interface IParameter
	{
		IImmutableList<Attribute> Attributes { get; }

		CollectionType CollectionType { get; }

		object? DefaultValue { get; }

		bool HasDefaultValue { get; }

		bool IsNullable { get; }

		bool IsOptional { get; }

		bool IsOut { get; }

		bool IsTask { get; }

		bool IsValueTask { get; }

		string Name { get; }

		NativeType NativeType { get; }

		RuntimeTypeHandle TypeHandle { get; }

		bool Supports(Type type);
	}
}
