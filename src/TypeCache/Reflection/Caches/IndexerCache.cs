// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;
using TypeCache.Extensions;
using TypeCache.Reflection.Members;

namespace TypeCache.Reflection.Caches
{
	internal sealed class IndexerCache<T> : IIndexerCache<T>
		where T : class
	{
		public IndexerCache()
		{
			this.Indexers = typeof(T).GetProperties(TypeCache.INSTANCE_BINDING)
				.If(propertyInfo => propertyInfo.GetIndexParameters().Any())
				.To(propertyInfo => (IIndexerMember)new IndexerMember(propertyInfo))
				.ToImmutable();
		}

		public IImmutableList<IIndexerMember> Indexers { get; }
	}
}
