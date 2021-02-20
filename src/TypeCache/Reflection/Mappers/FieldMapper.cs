// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Mappers
{
	internal class FieldMapper<FROM, TO> : IFieldMapper<FROM, TO>
		where FROM : notnull
		where TO : notnull
	{
		public IImmutableList<MapperSetting> Settings { get; }

		public FieldMapper(params MapperSetting[] overrides)
		{
			var settings = new Dictionary<string, MapperSetting>(TypeOf<TO>.Fields.Count, StringComparer.Ordinal);
			var fields = TypeOf<TO>.Fields.Keys.Match(TypeOf<FROM>.Fields.Keys, StringComparer.Ordinal);
			fields.Do(name =>
			{
				var fromField = TypeOf<FROM>.Fields[name];
				var toField = TypeOf<TO>.Fields[name];

				if (toField.Type == fromField.Type)
					settings.Add(name, new MapperSetting
					{
						From = name,
						To = name,
						IgnoreNullValue = !toField.Type.IsNullable
					});
			});

			overrides.Do(setting =>
			{
				var toField = TypeOf<TO>.Fields.GetValue(setting.To) switch
				{
					FieldMember member when member.SetValue != null => member,
					FieldMember _ => throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.To)} field [{setting.To}] is not writable."),
					_ => throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.To)} field [{setting.To}] was not found for mapping.")
				};

				if (!setting.From.IsBlank())
				{
					var fromField = TypeOf<FROM>.Fields.GetValue(setting.From) switch
					{
						FieldMember member when member.GetValue != null => member,
						FieldMember _ => throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.From)} field [{setting.From}] is not readable."),
						_ => throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.From)} field [{setting.From}] was not found for mapping.")
					};

					if (fromField.Type == toField.Type)
						settings[setting.To] = setting;
					else if (fromField.Type.NativeType == NativeType.None || toField.Type.NativeType == NativeType.None)
					{
						var fromTypeName = fromField.Type.Name;
						var toTypeName = toField.Type.Name;
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
			var fromField = TypeOf<FROM>.Fields[setting.From];
			var toField = TypeOf<TO>.Fields[setting.To];

			var fromValue = fromField.GetValue!(from);
			if (!setting.IgnoreNullValue || fromValue != null)
				toField.SetValue!(to, fromValue);
		});
	}
}
