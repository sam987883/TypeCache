// Copyright (c) 2020 Samuel Abraham

using System.Collections.Immutable;

namespace Sam987883.Reflection
{
	public interface IIndexerCache<T>
		where T : class
	{
		IImmutableList<IIndexerMember<T>> Indexers { get; }
	}
}
