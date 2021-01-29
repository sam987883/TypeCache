// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection.Mappers
{
	internal class PropertyMapper<FROM, TO> : IPropertyMapper<FROM, TO>
		where FROM : notnull
		where TO : notnull
	{
		public IImmutableList<MapperSetting> Settings { get; }

		public PropertyMapper(params MapperSetting[] overrides)
		{
			var settings = new Dictionary<string, MapperSetting>(TypeOf<TO>.Properties.Count, StringComparer.Ordinal);
			var properties = TypeOf<TO>.Properties.Keys.Match(TypeOf<FROM>.Properties.Keys, StringComparer.Ordinal);
			properties.Do(name =>
			{
				var fromProperty = TypeOf<FROM>.Properties[name];
				var toProperty = TypeOf<TO>.Properties[name];

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
				var toProperty = TypeOf<TO>.Properties.Get(setting.To);
				if (toProperty == null)
					throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.To)} property [{setting.To}] was not found for mapping.");
				else if (toProperty.Setter == null)
					throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.To)} property [{setting.To}] is not writable.");

				if (!setting.From.IsBlank())
				{
					var fromProperty = TypeOf<FROM>.Properties.Get(setting.From);
					if (fromProperty == null)
						throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.From)} property [{setting.From}] was not found for mapping.");
					else if (fromProperty.Getter == null)
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
			var fromProperty = TypeOf<FROM>.Properties[setting.From];
			var toProperty = TypeOf<TO>.Properties[setting.To];

			var fromValue = fromProperty[from];
			if (!setting.IgnoreNullValue || fromValue != null)
				toProperty[to] = fromValue;
		});
	}
}
