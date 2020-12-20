// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using TypeCache.Common;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Mappers
{
	internal class PropertyMapper<FROM, TO> : IPropertyMapper<FROM, TO>
		where FROM : class
		where TO : class
	{
		private readonly IPropertyCache<FROM> _FromPropertyCache;

		private readonly IPropertyCache<TO> _ToPropertyCache;

		public IImmutableList<MapperSetting> Settings { get; }

		public PropertyMapper(IPropertyCache<FROM> fromPropertyCache, IPropertyCache<TO> toPropertyCache, params MapperSetting[] overrides)
		{
			this._FromPropertyCache = fromPropertyCache;
			this._ToPropertyCache = toPropertyCache;

			var settings = new Dictionary<string, MapperSetting>(toPropertyCache.Properties.Count, Caches.TypeCache.NameComparer);
			var properties = this._ToPropertyCache.Properties.Keys.Match(this._FromPropertyCache.Properties.Keys, Caches.TypeCache.NameComparer);
			properties.Do(name =>
			{
				var fromProperty = this._FromPropertyCache.Properties[name];
				var toProperty = this._ToPropertyCache.Properties[name];

				if (toProperty.TypeHandle.Equals(fromProperty.TypeHandle))
					settings.Add(name, new MapperSetting
					{
						From = name,
						To = name,
						IgnoreNullValue = !toProperty.IsNullable
					});
			});

			overrides.Do(setting =>
			{
				var toProperty = this._ToPropertyCache.Properties.Get(setting.To);
				if (toProperty != null)
					throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.To)} property [{setting.To}] was not found for mapping.");
				else if (toProperty.SetMethod == null)
					throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.To)} property [{setting.To}] is not writable.");

				if (!setting.From.IsBlank())
				{
					var fromProperty = this._FromPropertyCache.Properties.Get(setting.From);
					if (fromProperty != null)
						throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.From)} property [{setting.From}] was not found for mapping.");
					else if (fromProperty.GetMethod == null)
						throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.From)} property [{setting.From}] is not writable.");

					if (fromProperty.TypeHandle.Equals(toProperty.TypeHandle))
						settings[setting.To] = setting;
					else if (fromProperty.NativeType == NativeType.Object || toProperty.NativeType == NativeType.Object)
					{
						var fromTypeName = fromProperty.TypeHandle.ToType().Name;
						var toTypeName = toProperty.TypeHandle.ToType().Name;
						throw new ArgumentOutOfRangeException(nameof(overrides), $"Property [{setting.From}] of type {fromTypeName} cannot be mapped to [{setting.To}] of type {toTypeName}.");
					}
				}
				else
					settings.Remove(setting.To);
			});
			this.Settings = settings.Values.ToImmutable();
		}

		public void Map(FROM from, TO to) => this.Settings.Do(setting =>
		{
			var fromProperty = this._FromPropertyCache.Properties[setting.From];
			var toProperty = this._ToPropertyCache.Properties[setting.To];

			var fromValue = fromProperty[from];
			if (!setting.IgnoreNullValue || fromValue != null)
				toProperty[to] = fromValue;
		});
	}
}
