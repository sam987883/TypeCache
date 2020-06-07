// Copyright (c) 2020 Samuel Abraham

using sam987883.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace sam987883.Reflection
{
	internal sealed class PropertyAccessor<T> : IPropertyAccessor<T>
	{
		private readonly T _Instance;
		private readonly IPropertyCache<T> _PropertyCache;

		public PropertyAccessor(IPropertyCache<T> propertyCache, T instance)
		{
			this._Instance = instance;
			this._PropertyCache = propertyCache;
		}

		public object? this[string key]
		{
			get => this._PropertyCache.Properties.Get(key).Value?.GetMethod?.Invoke(this._Instance);
			set => this._PropertyCache.Properties.Get(key).Value?.SetMethod?.Invoke(this._Instance, value);
		}

		public IEnumerable<string> Keys =>
			this._PropertyCache.Properties.Keys;

		public IEnumerable<object?> Values =>
			this._PropertyCache.Properties.Keys.To(key => this._PropertyCache.Properties[key].GetMethod?.Invoke(this._Instance));

		public int Count =>
			this._PropertyCache.Properties.Count;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ContainsKey(string key) =>
			this._PropertyCache.Properties.ContainsKey(key);

		public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() =>
			this._PropertyCache.Properties
				.To(pair => new KeyValuePair<string, object?>(pair.Key, pair.Value.GetMethod?.Invoke(this._Instance)))
				.GetEnumerator();

		public bool TryGetValue(string key, out object? value)
		{
			if (this._PropertyCache.Properties.TryGetValue(key, out var propertyMember) && propertyMember.GetMethod != null)
			{
				value = propertyMember.GetMethod.Invoke(this._Instance);
				return true;
			}
			value = null;
			return false;
		}

		IEnumerator IEnumerable.GetEnumerator() =>
			this._PropertyCache.Properties
				.To(pair => new KeyValuePair<string, object?>(pair.Key, pair.Value.GetMethod?.Invoke(this._Instance)))
				.GetEnumerator();
	}
}
