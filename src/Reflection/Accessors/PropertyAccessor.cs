// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common.Extensions;
using Sam987883.Reflection.Caches;
using System.Collections.Immutable;

namespace Sam987883.Reflection.Accessors
{
	internal sealed class PropertyAccessor<T> : IMemberAccessor
		where T : class
	{
		private readonly T _Instance;
		private readonly IPropertyCache<T> _PropertyCache;

		public PropertyAccessor(IPropertyCache<T> propertyCache, T instance)
		{
			this._Instance = instance;
			this._PropertyCache = propertyCache;
		}

		public object? this[string name]
		{
			get => this._PropertyCache.Properties.Get(name).Value[this._Instance];
			set => this._PropertyCache.Properties.Get(name).Value[this._Instance] = value;
		}

		public IImmutableList<string> GetNames => this._PropertyCache.GetNames;

		public IImmutableList<string> SetNames => this._PropertyCache.SetNames;

		public IImmutableDictionary<string, object?> Values
		{
			get => this._PropertyCache.Properties
				.GetValues(this.GetNames)
				.ToImmutableDictionary(_ => _.Name, _ => this._PropertyCache.Properties[_.Name][this._Instance], TypeCache.NameComparer);
		}
	}
}
