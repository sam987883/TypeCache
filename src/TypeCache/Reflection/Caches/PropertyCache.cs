// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Reflection.Accessors;
using TypeCache.Reflection.Members;

namespace TypeCache.Reflection.Caches
{
	internal sealed class PropertyCache<T> : IPropertyCache<T>
	{
		public PropertyCache()
		{
			this.Properties = typeof(T).GetProperties(TypeCache.INSTANCE_BINDING)
				.If(propertyInfo => !propertyInfo.GetIndexParameters().Any())
				.To(propertyInfo => KeyValuePair.Create(propertyInfo.Name, (IPropertyMember)new PropertyMember(propertyInfo)))
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

		public IImmutableDictionary<string, IPropertyMember> Properties { get; }

		public IImmutableList<string> GetNames { get; }

		public IImmutableList<string> SetNames { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IMemberAccessor CreateAccessor(object instance)
			=> new PropertyAccessor((IPropertyCache<object>)(IPropertyCache<T>)this, instance);

		public void Map(object from, object to) =>
			this.Properties.Values
				.If(property => property.Public && property.GetMethod != null && property.SetMethod != null)
				.Do(property => property[to] = property[from]);

		public void Map(IDictionary<string, object> from, object to, IEqualityComparer<string>? comparer = null)
		{
			comparer ??= TypeCache.NameComparer;

			from.If(pair => this.SetNames.Has(pair.Key, comparer)).Do(pair => this.Properties[pair.Key][to] = pair.Value);
		}

		public void Map(object from, IDictionary<string, object?> to, IEqualityComparer<string>? comparer = null)
		{
			comparer ??= TypeCache.NameComparer;

			to.Keys.If(name => this.GetNames.Has(name, comparer)).Do(name => to[name] = this.Properties[name][from]);
		}
	}
}
