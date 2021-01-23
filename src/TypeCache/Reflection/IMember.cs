﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;

namespace TypeCache.Reflection
{
	public interface IMember
	{
		IImmutableList<Attribute> Attributes { get; }

		CollectionType CollectionType { get; }

		bool Internal { get; }

		bool IsNullable { get; }

		bool IsTask { get; }

		bool IsValueTask { get; }

		string Name { get; }

		bool Public { get; }

		NativeType NativeType { get; }

		RuntimeTypeHandle TypeHandle { get; }
	}
}
