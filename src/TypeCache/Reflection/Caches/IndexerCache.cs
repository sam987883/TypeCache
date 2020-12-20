// Copyright (c) 2021 Samuel Abraham

using TypeCache.Reflection.Members;
using System.Collections.Immutable;
using static TypeCache.Extensions.IEnumerableExtensions;

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
