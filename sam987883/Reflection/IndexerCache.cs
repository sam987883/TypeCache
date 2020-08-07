// Copyright (c) 2020 Samuel Abraham

using sam987883.Reflection.Members;
using System.Collections.Immutable;
using static sam987883.Common.Extensions.IEnumerableExtensions;

namespace sam987883.Reflection
{
	internal sealed class IndexerCache<T> : IIndexerCache<T>
		where T : class
	{
		public IndexerCache()
		{
			this.Indexers = typeof(T).GetProperties(TypeCache.INSTANCE_BINDING)
				.If(propertyInfo => propertyInfo.GetIndexParameters().Any())
				.To(propertyInfo => (IIndexerMember<T>)new IndexerMember<T>(propertyInfo))
				.ToImmutable();
		}

		public IImmutableList<IIndexerMember<T>> Indexers { get; }
	}
}
