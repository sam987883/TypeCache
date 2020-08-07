// Copyright (c) 2020 Samuel Abraham

using sam987883.Common.Extensions;
using System.Collections.Generic;

namespace sam987883.Reflection.Accessors
{
	internal sealed class FieldAccessor<T> : IFieldAccessor
		where T : class
	{
		private readonly T _Instance;
		private readonly IFieldCache<T> _FieldCache;

		public FieldAccessor(IFieldCache<T> fieldCache, T instance)
		{
			this._Instance = instance;
			this._FieldCache = fieldCache;
		}

		public object? this[string name]
		{
			get => this._FieldCache.Fields.Get(name).Value[this._Instance];
			set => this._FieldCache.Fields.Get(name).Value[this._Instance] = value;
		}

		public string[] Names =>
			this._FieldCache.Fields.Keys.ToArray(this._FieldCache.Fields.Count);

		public IDictionary<string, object?> Values
		{
			get => this._FieldCache.Fields.Keys.ToDictionary(name => this._FieldCache.Fields[name][this._Instance], TypeCache.NameComparer);
			set => value.Do(pair =>
			{
				var field = this._FieldCache.Fields.Get(pair.Key);
				if (field.Exists)
					field.Value[this._Instance] = pair.Value;
			});
		}
	}
}
