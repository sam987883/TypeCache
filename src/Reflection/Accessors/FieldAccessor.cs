// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common.Extensions;
using Sam987883.Reflection.Caches;
using System.Collections.Immutable;

namespace Sam987883.Reflection.Accessors
{
	internal sealed class FieldAccessor<T> : IMemberAccessor
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

		public IImmutableList<string> GetNames => this._FieldCache.GetNames;

		public IImmutableList<string> SetNames => this._FieldCache.SetNames;

		public IImmutableDictionary<string, object?> Values
		{
			get => this._FieldCache.Fields
				.GetValues(this.GetNames)
				.ToImmutableDictionary(_ => _.Name, _ => this._FieldCache.Fields[_.Name][this._Instance], TypeCache.NameComparer);
		}
	}
}
