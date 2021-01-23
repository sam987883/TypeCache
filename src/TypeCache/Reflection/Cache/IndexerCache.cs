// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Extensions;
using TypeCache.Reflection.Members;

namespace TypeCache.Reflection.Cache
{
	internal static class IndexerCache<T>
	{
		internal static IImmutableList<IIndexerMember> Indexers { get; }

		static IndexerCache()
		{
			Indexers = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.If(propertyInfo => propertyInfo.GetIndexParameters().Any())
				.To(propertyInfo => (IIndexerMember)new IndexerMember(propertyInfo))
				.ToImmutable();
		}
	}
}
