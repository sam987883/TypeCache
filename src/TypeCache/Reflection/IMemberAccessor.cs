// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;

namespace TypeCache.Reflection
{
	public interface IMemberAccessor
	{
		object? this[string name] { get; set; }

		IImmutableList<string> GetNames { get; }

		IImmutableList<string> SetNames { get; }

		IImmutableDictionary<string, object?> Values { get; }
	}
}
