// Copyright (c) 2020 Samuel Abraham

using sam987883.Common;
using sam987883.Reflection.Members;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using static sam987883.Extensions.IEnumerableExtensions;

namespace sam987883.Reflection
{
	internal sealed class PropertyCache<T> : IPropertyCache<T>
	{
		public PropertyCache()
		{
			var valueComparer = new CustomEqualityComparer<IPropertyMember<T>>((x, y) => TypeCache.NameComparer.Equals(x.Name, y.Name));
			this.Properties = typeof(T).GetProperties(TypeCache.INSTANCE_BINDING)
				.If(propertyInfo => !propertyInfo.GetIndexParameters().Any())
				.To(propertyInfo => KeyValuePair.Create(propertyInfo.Name, (IPropertyMember<T>)new PropertyMember<T>(propertyInfo)))
				.ToImmutable(TypeCache.NameComparer, valueComparer);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IPropertyAccessor<T> CreateAccessor(T instance) =>
			new PropertyAccessor<T>(this, instance);

		public IImmutableDictionary<string, IPropertyMember<T>> Properties { get; }

		public void Map(T from, T to) =>
			this.Properties.Values
				.If(property => property.Public && property.GetMethod != null && property.SetMethod != null)
				.Do(property => property[to] = property[from]);
	}
}
