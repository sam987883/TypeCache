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

				if (toProperty.PropertyType.Equals(fromProperty.PropertyType))
					settings.Add(name, new MapperSetting(name, name, !toProperty.PropertyType.IsNullable()));
			});

			overrides.Do(setting =>
			{
				var toProperty = TypeOf<TO>.Properties.GetValue(setting.To) switch
				{
					PropertyMember property when property.Setter is not null => property,
					_ => throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.To)} property [{setting.To}] is not writable.")
				};

				if (!setting.From.IsBlank())
				{
					var fromProperty = TypeOf<FROM>.Properties.GetValue(setting.From) switch
					{
						PropertyMember property when property.Getter is not null => property,
						_ => throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.From)} property [{setting.From}] is not readable.")
					};

					if (fromProperty.PropertyType.Equals(toProperty.PropertyType))
						settings[setting.To] = setting;
					else if (fromProperty.PropertyType.SystemType == SystemType.Unknown || toProperty.PropertyType.SystemType == SystemType.Unknown)
					{
						var fromTypeName = fromProperty.PropertyType.Name;
						var toTypeName = toProperty.PropertyType.Name;
						throw new ArgumentOutOfRangeException(nameof(overrides), $"Property [{setting.From}] of type {fromTypeName} cannot be mapped to [{setting.To}] of type {toTypeName}.");
					}
				}
				else
					settings.Remove(setting.To);
			});
			this.Settings = settings.Values.ToImmutableArray();
		}

		public void Map(FROM from, TO to) => this.Settings.Do(setting =>
		{
			var fromProperty = TypeOf<FROM>.Properties[setting.From];
			var toProperty = TypeOf<TO>.Properties[setting.To];

			var fromValue = fromProperty.GetValue(from);
			if (!setting.IgnoreNullValue || fromValue is not null)
				toProperty.SetValue(to, fromValue);
		});
	}
}
