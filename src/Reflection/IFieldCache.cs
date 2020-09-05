// Copyright (c) 2020 Samuel Abraham

using System.Collections.Immutable;

namespace Sam987883.Reflection
{
	public interface IFieldCache<T>
		where T : class
	{
		void Map(T from, T to);

		IImmutableDictionary<string, IFieldMember<T>> Fields { get; }
	}
}
