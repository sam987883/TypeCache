﻿// Copyright (c) 2020 Samuel Abraham

using sam987883.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using static sam987883.Extensions.IEnumerableExtensions;
using static sam987883.Extensions.IReadOnlyDictionaryExtensions;

namespace sam987883.Reflection.Mappers
{
	internal class PropertyMapper<FROM, TO> : IPropertyMapper<FROM, TO>
	{
		private readonly IPropertyCache<FROM> _FromPropertyCache;

		private readonly IPropertyCache<TO> _ToPropertyCache;

		public IImmutableList<MapperSetting> Settings { get; }

		public PropertyMapper(IPropertyCache<FROM> fromPropertyCache, IPropertyCache<TO> toPropertyCache, params MapperSetting[] overrides)
		{
			this._FromPropertyCache = fromPropertyCache;
			this._ToPropertyCache = toPropertyCache;

			var settings = new Dictionary<string, MapperSetting>(toPropertyCache.Properties.Count, TypeCache.NameComparer);
			var properties = this._ToPropertyCache.Properties.Keys.Match(this._FromPropertyCache.Properties.Keys, TypeCache.NameComparer);
			properties.Do(name =>
			{
				var fromProperty = this._FromPropertyCache.Properties[name];
				var toProperty = this._ToPropertyCache.Properties[name];

				if (toProperty.TypeHandle.Equals(fromProperty.TypeHandle))
					settings.Add(name, new MapperSetting(name, name, !toProperty.IsNullable));
			});

			overrides.Do(setting =>
			{
				var (toProperty, toExists) = this._ToPropertyCache.Properties.Get(setting.To);
				if (!toExists)
					throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.To)} property [{setting.To}] was not found for mapping.");

				if (!setting.From.IsBlank())
				{
					var (fromProperty, fromExists) = this._FromPropertyCache.Properties.Get(setting.From);
					if (!fromExists)
						throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.From)} property [{setting.From}] was not found for mapping.");

					if (toProperty.TypeHandle.Equals(fromProperty.TypeHandle))
						settings[setting.To] = setting;
					else
					{
						var fromTypeName = Type.GetTypeFromHandle(fromProperty.TypeHandle).Name;
						var toTypeName = Type.GetTypeFromHandle(toProperty.TypeHandle).Name;
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