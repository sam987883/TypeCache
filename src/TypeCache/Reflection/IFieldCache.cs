// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Collections.Immutable;

namespace TypeCache.Reflection
{
	public interface IFieldCache<out T>
	{
		IImmutableDictionary<string, IFieldMember> Fields { get; }

		IImmutableList<string> GetNames { get; }

		IImmutableList<string> SetNames { get; }

		IMemberAccessor CreateAccessor(object instance);

		void Map(object from, object to);

		void Map(IDictionary<string, object> from, object to, IEqualityComparer<string>? comparer = null);

		void Map(object from, IDictionary<string, object?> to, IEqualityComparer<string>? comparer = null);
	}
}
