// Copyright (c) 2020 Samuel Abraham

using sam987883.Common.Extensions;
using System.Collections.Generic;

namespace sam987883.Reflection
{
	internal sealed class PropertyAccessor<T> : IPropertyAccessor<T>
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

		public string[] Names => this._PropertyCache.Properties.Keys.ToArray(this._PropertyCache.Properties.Count);

		public IDictionary<string, object?> Values
		{
			get => this._PropertyCache.Properties.Keys.ToDictionary(name => this._PropertyCache.Properties[name][this._Instance], TypeCache.NameComparer);
			set => value.Do(pair =>
			{
				var property = this._PropertyCache.Properties.Get(pair.Key);
				if (property.Exists)
					property.Value[this._Instance] = pair.Value;
			});
		}
	}
}
