// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;

namespace TypeCache.Reflection;

public interface IMember
{
	public IImmutableList<Attribute> Attributes { get; }

	public bool Internal { get; }

	public string Name { get; }

	public bool Public { get; }
}
