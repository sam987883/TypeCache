// Copyright (c) 2020 Samuel Abraham

using sam987883.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace sam987883.Reflection
{
	internal sealed class FieldAccessor<T> : IFieldAccessor<T>
	{
		private readonly T _Instance;
		private readonly IFieldCache<T> _FieldCache;

		public FieldAccessor(IFieldCache<T> fieldCache, T instance)
		{
			this._Instance = instance;
			this._FieldCache = fieldCache;
		}

		public object? this[string key]
		{
			get => this._FieldCache.Fields.Get(key).Value[this._Instance];
			set => this._FieldCache.Fields.Get(key).Value[this._Instance] = value;
		}

		public IEnumerable<string> Keys =>
			this._FieldCache.Fields.Keys;

		public IEnumerable<object?> Values =>
			this._FieldCache.Fields.Keys.To(key => this._FieldCache.Fields[key][this._Instance]);

		public int Count =>
			this._FieldCache.Fields.Count;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ContainsKey(string key) =>
			this._FieldCache.Fields.ContainsKey(key);

		public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() =>
			this._FieldCache.Fields
				.To(pair => new KeyValuePair<string, object?>(pair.Key, pair.Value[this._Instance]))
				.GetEnumerator();

		public bool TryGetValue(string key, out object? value)
		{
			if (this._FieldCache.Fields.TryGetValue(key, out var fieldMember))
			{
				value = fieldMember[this._Instance];
				return true;
			}
			value = null;
			return false;
		}

		IEnumerator IEnumerable.GetEnumerator() =>
			this._FieldCache.Fields
				.To(pair => new KeyValuePair<string, object?>(pair.Key, pair.Value[this._Instance]))
				.GetEnumerator();
	}
}
