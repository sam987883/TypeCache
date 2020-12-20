// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Accessors
{
	internal sealed class PropertyAccessor : IMemberAccessor
	{
		private readonly object _Instance;
		private readonly IPropertyCache<object> _PropertyCache;

		public PropertyAccessor(IPropertyCache<object> propertyCache, object instance)
		{
			this._Instance = instance;
			this._PropertyCache = propertyCache;
		}

		public object? this[string name]
		{
			get => this._PropertyCache.Properties.Get(name)?[this._Instance];
			set
			{
				var property = this._PropertyCache.Properties.Get(name);
				if (property != null)
					property[this._Instance] = value;
			}
		}

		public IImmutableList<string> GetNames => this._PropertyCache.GetNames;

		public IImmutableList<string> SetNames => this._PropertyCache.SetNames;

		public IImmutableDictionary<string, object?> Values
			=> this._PropertyCache.Properties
				.GetValues(this.GetNames)
				.ToImmutableDictionary(_ => _.Name, _ => this._PropertyCache.Properties[_.Name][this._Instance], Caches.TypeCache.NameComparer);
	}
}
