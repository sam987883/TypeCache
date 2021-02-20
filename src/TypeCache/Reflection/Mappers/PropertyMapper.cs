﻿// Copyright (c) 2021 Samuel Abraham

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

				if (toProperty.Type.Equals(fromProperty.Type))
					settings.Add(name, new MapperSetting
					{
						From = name,
						To = name,
						IgnoreNullValue = !toProperty.Type.IsNullable
					});
			});

			overrides.Do(setting =>
			{
				var toProperty = TypeOf<TO>.Properties.GetValue(setting.To) switch
				{
					PropertyMember property when property.Setter != null => property,
					PropertyMember property => throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.To)} property [{setting.To}] is not writable."),
					_ => throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.To)} property [{setting.To}] was not found for mapping.")
				};

				if (!setting.From.IsBlank())
				{
					var fromProperty = TypeOf<FROM>.Properties.GetValue(setting.From) switch
					{
						PropertyMember property when property.Getter != null => property,
						PropertyMember property => throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.From)} property [{setting.From}] is not readable."),
						_ => throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.From)} property [{setting.From}] was not found for mapping.")
					};

					if (fromProperty.Type.Equals(toProperty.Type))
						settings[setting.To] = setting;
					else if (fromProperty.Type.NativeType == NativeType.None || toProperty.Type.NativeType == NativeType.None)
					{
						var fromTypeName = fromProperty.Type.Name;
						var toTypeName = toProperty.Type.Name;
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

			var fromValue = fromProperty.GetValue(from);
			if (!setting.IgnoreNullValue || fromValue != null)
				toProperty.SetValue(to, fromValue);
		});
	}
}
