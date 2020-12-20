// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;

namespace TypeCache.Reflection
{
	public interface IIndexerCache<out T>
		where T : class
	{
		IImmutableList<IIndexerMember> Indexers { get; }
	}
}
