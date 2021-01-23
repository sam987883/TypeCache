// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Mappers
{
	internal class FieldMapper<FROM, TO> : IFieldMapper<FROM, TO>
		where FROM : class
		where TO : class
	{
		public IImmutableList<MapperSetting> Settings { get; }

		public FieldMapper(params MapperSetting[] overrides)
		{
			var settings = new Dictionary<string, MapperSetting>(Class<TO>.Fields.Count, StringComparer.Ordinal);
			var fields = Class<TO>.Fields.Keys.Match(Class<FROM>.Fields.Keys, StringComparer.Ordinal);
			fields.Do(name =>
			{
				var fromField = Class<FROM>.Fields[name];
				var toField = Class<TO>.Fields[name];

				if (toField.TypeHandle.Equals(fromField.TypeHandle))
					settings.Add(name, new MapperSetting
					{
						From = name,
						To = name,
						IgnoreNullValue = !toField.IsNullable
					});
			});

			overrides.Do(setting =>
			{
				var toField = Class<TO>.Fields.Get(setting.To);
				if (toField == null)
					throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.To)} field [{setting.To}] was not found for mapping.");
				else if (toField.SetValue == null)
					throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.To)} field [{setting.To}] is not writable.");

				if (!setting.From.IsBlank())
				{
					var fromField = Class<FROM>.Fields.Get(setting.From);
					if (fromField == null)
						throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.From)} field [{setting.From}] was not found for mapping.");
					else if (fromField.GetValue == null)
						throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.From)} field [{setting.From}] is not writable.");

					if (fromField.TypeHandle.Equals(toField.TypeHandle))
						settings[setting.To] = setting;
					else if (fromField.NativeType == NativeType.Object || toField.NativeType == NativeType.Object)
					{
						var fromTypeName = fromField.TypeHandle.ToType().Name;
						var toTypeName = toField.TypeHandle.ToType().Name;
						throw new ArgumentOutOfRangeException(nameof(overrides), $"Field [{setting.From}] of type {fromTypeName} cannot be mapped to [{setting.To}] of type {toTypeName}.");
					}
				}
				else
					settings.Remove(setting.To);
			});
			this.Settings = settings.Values.ToImmutable();
		}

		public void Map(FROM from, TO to) => this.Settings.Do(setting =>
		{
			var fromField = Class<FROM>.Fields[setting.From];
			var toField = Class<TO>.Fields[setting.To];

			var fromValue = fromField[from];
			if (!setting.IgnoreNullValue || fromValue != null)
				toField[to] = fromValue;
		});
	}
}
