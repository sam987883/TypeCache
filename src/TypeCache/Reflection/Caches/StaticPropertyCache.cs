// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Collections.Immutable;
using TypeCache.Common;
using TypeCache.Extensions;
using TypeCache.Reflection.Members;

namespace TypeCache.Reflection.Caches
{
	internal sealed class StaticPropertyCache<T> : IStaticPropertyCache<T>
		where T : class
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
