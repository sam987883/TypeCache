// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using TypeCache.Common;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Mappers
{
	internal class FieldMapper<FROM, TO> : IFieldMapper<FROM, TO>
		where FROM : class
		where TO : class
	{
		private readonly IFieldCache<FROM> _FromFieldCache;

		private readonly IFieldCache<TO> _ToFieldCache;

		public IImmutableList<MapperSetting> Settings { get; }

		public FieldMapper(IFieldCache<FROM> fromFieldCache, IFieldCache<TO> toFieldCache, params MapperSetting[] overrides)
		{
			this._FromFieldCache = fromFieldCache;
			this._ToFieldCache = toFieldCache;

			var settings = new Dictionary<string, MapperSetting>(toFieldCache.Fields.Count, Caches.TypeCache.NameComparer);
			var fields = this._ToFieldCache.Fields.Keys.Match(this._FromFieldCache.Fields.Keys, Caches.TypeCache.NameComparer);
			fields.Do(name =>
			{
				var fromField = this._FromFieldCache.Fields[name];
				var toField = this._ToFieldCache.Fields[name];

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
				var toField = this._ToFieldCache.Fields.Get(setting.To);
				if (toField == null)
					throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.To)} field [{setting.To}] was not found for mapping.");
				else if (toField.SetValue == null)
					throw new ArgumentOutOfRangeException(nameof(overrides), $"{nameof(setting.To)} field [{setting.To}] is not writable.");

				if (!setting.From.IsBlank())
				{
					var fromField = this._FromFieldCache.Fields.Get(setting.From);
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
			var fromField = this._FromFieldCache.Fields[setting.From];
			var toField = this._ToFieldCache.Fields[setting.To];

			var fromValue = fromField[from];
			if (!setting.IgnoreNullValue || fromValue != null)
				toField[to] = fromValue;
		});
	}
}
