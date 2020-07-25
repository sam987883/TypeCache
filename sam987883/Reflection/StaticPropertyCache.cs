// Copyright (c) 2020 Samuel Abraham

using sam987883.Common;
using sam987883.Reflection.Members;
using System.Collections.Generic;
using System.Collections.Immutable;
using static sam987883.Common.Extensions.IEnumerableExtensions;

namespace sam987883.Reflection
{
	internal sealed class StaticPropertyCache<T> : IStaticPropertyCache<T>
	{
		public StaticPropertyCache()
		{
			var valueComparer = new CustomEqualityComparer<IStaticPropertyMember>((x, y) => TypeCache.NameComparer.Equals(x.Name, y.Name));
			this.Properties = typeof(T).GetProperties(TypeCache.STATIC_BINDING)
				.To(propertyInfo => KeyValuePair.Create(propertyInfo.Name, (IStaticPropertyMember)new StaticPropertyMember(propertyInfo)))
				.ToImmutable(TypeCache.NameComparer, valueComparer);
		}

		public IImmutableDictionary<string, IStaticPropertyMember> Properties { get; }
	}
}
