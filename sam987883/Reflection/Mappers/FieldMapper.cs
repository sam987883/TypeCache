// Copyright (c) 2020 Samuel Abraham

using sam987883.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace sam987883.Reflection.Mappers
{
	internal class FieldMapper<FROM, TO> : IFieldMapper<FROM, TO>
	{
		private readonly IFieldCache<FROM> _FromFieldCache;

		private readonly IFieldCache<TO> _ToFieldCache;

		public IImmutableList<MapperSetting> Settings { get; }

		public FieldMapper(IFieldCache<FROM> fromFieldCache, IFieldCache<TO> toFieldCache, params MapperSetting[] overrides)
		{
			this._FromFieldCache = fromFieldCache;
			this._ToFieldCache = toFieldCache;

			var settings = new Dictionary<string, MapperSetting>(toFieldCache.Fields.Count, TypeCache.NameComparer);
			var properties = this._ToFieldCache.Fields.Keys.Match(this._FromFieldCache.Fields.Keys, TypeCache.NameComparer);
			properties.Do(name =>
			{
				var fromField = this._FromFieldCache.Fields[name];
				var toField = this._ToFieldCache.Fields[name];

				if (toField.TypeHandle.Equals(fromField.TypeHandle))
					settings.Add(name, new MapperSetting(name, name, !toField.IsNullable));
			});

			overrides.Do(setting =>
			{
				var (toField, toExists) = this._ToFieldCache.Fields.Get(setting.To);
				if (!toExists)
					throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.To)} field [{setting.To}] was not found for mapping.");

				if (!setting.From.IsBlank())
				{
					var (fromField, fromExists) = this._FromFieldCache.Fields.Get(setting.From);
					if (!fromExists)
						throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.From)} field [{setting.From}] was not found for mapping.");

					if (toField.TypeHandle.Equals(fromField.TypeHandle))
						settings[setting.To] = setting;
					else
					{
						var fromTypeName = Type.GetTypeFromHandle(fromField.TypeHandle).Name;
						var toTypeName = Type.GetTypeFromHandle(toField.TypeHandle).Name;
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
			var fromField = this._FromFieldCache.Fields[setting.From];
			var toField = this._ToFieldCache.Fields[setting.To];

			var fromValue = fromField[from];
			if (!setting.IgnoreNullValue || fromValue != null)
				toField[to] = fromValue;
		});
	}
}
