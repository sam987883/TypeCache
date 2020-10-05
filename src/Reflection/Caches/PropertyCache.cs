﻿// Copyright (c) 2020 Samuel Abraham

using Sam987883.Reflection.Members;
using System.Collections.Generic;
using System.Collections.Immutable;
using static Sam987883.Common.Extensions.IEnumerableExtensions;

namespace Sam987883.Reflection.Caches
{
	internal sealed class PropertyCache<T> : IPropertyCache<T>
		where T : class
	{
		public PropertyCache()
		{
			this.Properties = typeof(T).GetProperties(TypeCache.INSTANCE_BINDING)
				.If(propertyInfo => !propertyInfo.GetIndexParameters().Any())
				.To(propertyInfo => KeyValuePair.Create(propertyInfo.Name, (IPropertyMember<T>)new PropertyMember<T>(propertyInfo)))
				.ToImmutable(TypeCache.NameComparer);
			this.GetNames = this.Properties
				.If(_ => _.Value.GetMethod != null)
				.To(_ => _.Value.Name)
				.ToImmutableArray();
			this.SetNames = this.Properties
				.If(_ => _.Value.SetMethod != null)
				.To(_ => _.Value.Name)
				.ToImmutableArray();
		}

		public IImmutableDictionary<string, IPropertyMember<T>> Properties { get; }

		public IImmutableList<string> GetNames { get; }

		public IImmutableList<string> SetNames { get; }

		public void Map(T from, T to) =>
			this.Properties.Values
				.If(property => property.Public && property.GetMethod != null && property.SetMethod != null)
				.Do(property => property[to] = property[from]);
	}
}
