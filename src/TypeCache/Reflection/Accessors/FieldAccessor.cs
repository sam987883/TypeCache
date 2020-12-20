// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Accessors
{
	internal sealed class FieldAccessor : IMemberAccessor
	{
		private readonly object _Instance;
		private readonly IFieldCache<object> _FieldCache;

		public FieldAccessor(IFieldCache<object> fieldCache, object instance)
		{
			this._Instance = instance;
			this._FieldCache = fieldCache;
		}

		public object? this[string name]
		{
			get => this._FieldCache.Fields.Get(name)?[this._Instance];
			set
			{
				var field = this._FieldCache.Fields.Get(name);
				if (field != null)
					field[this._Instance] = value;
			}
		}

		public IImmutableList<string> GetNames => this._FieldCache.GetNames;

		public IImmutableList<string> SetNames => this._FieldCache.SetNames;

		public IImmutableDictionary<string, object?> Values
			=> this._FieldCache.Fields
				.GetValues(this.GetNames)
				.ToImmutableDictionary(_ => _.Name, _ => this._FieldCache.Fields[_.Name][this._Instance], Caches.TypeCache.NameComparer);
	}
}
