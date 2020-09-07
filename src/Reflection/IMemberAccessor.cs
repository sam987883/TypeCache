// Copyright (c) 2020 Samuel Abraham

using System.Collections.Immutable;

namespace Sam987883.Reflection
{
	public interface IMemberAccessor
	{
		object? this[string name] { get; set; }

		IImmutableList<string> GetNames { get; }

		IImmutableList<string> SetNames { get; }

		IImmutableDictionary<string, object?> Values { get; }
	}
}
