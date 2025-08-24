// Copyright (c) 2021 Samuel Abraham

using TypeCache.Adapters;
using TypeCache.Attributes;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Mapping;

internal class FieldMapper : IMapper
{
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public void Map<T>(IEnumerable<KeyValuePair<string, object?>> source, T target)
		where T : class
	{
		source.ThrowIfNull();
		target.ThrowIfNull();

		Type<T>.CollectionType.ThrowIfNotEqual(CollectionType.None, Invariant($"Cannot map to a collection: {Type<T>.CollectionType.Name()}"));

		var targetFields = Type<T>.Fields;
		foreach (var pair in source)
		{
			if (!targetFields.TryGetValue(pair.Key, out var field))
				continue;

			if (pair.Value is null)
			{
				if (!field.FieldType.IsNullable())
					continue;

				field.SetValue(target, null);
				continue;
			}

			if (field.FieldType.ScalarType() is not ScalarType.None || field.FieldType.IsValueType)
			{
				field.SetValue(target, pair.Value);
				continue;
			}

			var targetValue = field.GetValue(target);
			if (targetValue is null)
			{
				targetValue = field.FieldType.Create();
				if (targetValue is null)
					continue;

				field.SetValue(target, targetValue);
			}

			this.Map(pair.Value, targetValue);
		}
	}

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public void Map<T>(IDictionary<string, object?> source, T target)
		where T : class
		=> this.Map(source as IEnumerable<KeyValuePair<string, object?>>, target);

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public void Map<S>(S source, IDictionary<string, object?> target)
		where S : class
	{
		source.ThrowIfNull();
		target.ThrowIfNull();

		Type<S>.ScalarType.ThrowIfNotEqual(ScalarType.None, Invariant($"Cannot map from a scalar: {Type<S>.ScalarType.Name()}"));
		Type<S>.CollectionType.ThrowIfNotEqual(CollectionType.None, Invariant($"Cannot map from a collection: {Type<S>.CollectionType.Name()}"));

		foreach (var sourceField in Type<S>.Fields.Values)
		{
			if (sourceField.Attributes.OfType<MapIgnoreAttribute>()
				.Any(_ => _.Type is null || _.Type == target.GetType()))
				continue;

			var value = sourceField.GetValue(source);
			var mapAttributes = sourceField.Attributes.OfType<MapAttribute>()
				.Where(_ => _.Type == typeof(IDictionary<string, object?>) || _.Type == target.GetType());
			if (mapAttributes.Any())
			{
				foreach (var attribute in mapAttributes)
					target[attribute.Member] = value;
			}
			else
			{
				target[sourceField.Name] = value;
			}
		}
	}

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public void Map<S, T>(S source, T target)
		where S : class
		where T : class
	{
		source.ThrowIfNull();
		target.ThrowIfNull();

		Type<S>.ScalarType.ThrowIfNotEqual(ScalarType.None, Invariant($"Cannot map from a scalar: {Type<S>.ScalarType.Name()}"));
		Type<T>.ScalarType.ThrowIfNotEqual(ScalarType.None, Invariant($"Cannot map to a scalar: {Type<T>.ScalarType.Name()}"));

		Type<S>.CollectionType.ThrowIfNotEqual(CollectionType.None, Invariant($"Cannot map from a collection: {Type<S>.CollectionType.Name()}"));
		Type<T>.CollectionType.ThrowIfNotEqual(CollectionType.None, Invariant($"Cannot map to a collection: {Type<T>.CollectionType.Name()}"));

		foreach (var sourceField in Type<S>.Fields.Values)
		{
			if (sourceField.Attributes.OfType<MapIgnoreAttribute>()
				.Any(_ => _.Type is null || _.Type == target.GetType()))
				continue;

			var sourceValue = sourceField.GetValue(source);
			var mapAttributes = sourceField.Attributes.OfType<MapAttribute>()
				.Where(_ => _.Type == target.GetType());
			if (mapAttributes.Any())
			{
				foreach (var attribute in mapAttributes)
					setFieldValue(attribute.Member, sourceValue);
			}
			else if (sourceField.FieldType.ScalarType() is not ScalarType.None
				|| sourceField.FieldType.IsValueType)
				setFieldValue(sourceField.Name, sourceValue);
		}

		void setFieldValue(string fieldName, object? value)
		{
			var field = Type<T>.Fields[fieldName];

			if (value is null)
			{
				if (!field.FieldType.IsNullable())
					return;

				field.SetValue(target, null);
				return;
			}

			if (field.FieldType.IsValueType || field.FieldType.ScalarType() is not ScalarType.None)
			{
				field.SetValue(target, value);
				return;
			}

			var targetValue = field.GetValue(target);
			if (targetValue is null)
			{
				targetValue = field.FieldType.Create();
				if (targetValue is null)
					return;

				field.SetValue(target, targetValue);
			}

			this.Map(value, targetValue);
		}
	}

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public void Map(object source, object target)
	{
		source.ThrowIfNull();
		target.ThrowIfNull();

		var sourceType = source.GetType();
		var targetType = target.GetType();

		sourceType.ScalarType().ThrowIfNotEqual(ScalarType.None, Invariant($"Cannot map from a scalar: {sourceType.ScalarType().Name()}"));
		targetType.ScalarType().ThrowIfNotEqual(ScalarType.None, Invariant($"Cannot map to a scalar: {targetType.ScalarType().Name()}"));

		var sourceCollectionType = sourceType.CollectionType();
		var targetCollectionType = targetType.CollectionType();

		Action? executeMap = (sourceCollectionType, targetCollectionType) switch
		{
			(CollectionType.Array, CollectionType.Array) =>
				() => this._Map((Array)source, (Array)target),
			(CollectionType.Array, CollectionType.List) =>
				() => this._Map((Array)source, new ListAdapter(target)),
			(CollectionType.Array, CollectionType.ReadOnlyList) =>
				() => this._Map((Array)source, new ReadOnlyListAdapter(target)),
			(CollectionType.Array, CollectionType.Collection or CollectionType.Set) =>
				() => this._Map((Array)source, new CollectionAdapter(target)),
			(CollectionType.List or CollectionType.ReadOnlyList, CollectionType.Array) =>
				() => this._Map(new ReadOnlyListAdapter(source), (Array)target),
			(CollectionType.List or CollectionType.ReadOnlyList, CollectionType.List) =>
				() => this._Map(new ReadOnlyListAdapter(source), new ListAdapter(target)),
			(CollectionType.List or CollectionType.ReadOnlyList, CollectionType.ReadOnlyList) =>
				() => this._Map(new ReadOnlyListAdapter(source), new ReadOnlyListAdapter(target)),
			(CollectionType.List or CollectionType.ReadOnlyList, CollectionType.Collection or CollectionType.Set) =>
				() => this._Map(new ReadOnlyListAdapter(source), new CollectionAdapter(target)),
			(CollectionType.Collection or CollectionType.Set, CollectionType.Collection or CollectionType.List or CollectionType.Set) =>
				() => this._Map(new CollectionAdapter(source), new CollectionAdapter(target)),
			(CollectionType.Dictionary, CollectionType.Dictionary) =>
				() => this._Map(new DictionaryAdapter(source), new DictionaryAdapter(target)),
			(CollectionType.ReadOnlyDictionary, CollectionType.Dictionary) =>
				() => this._Map(new ReadOnlyDictionaryAdapter(source), new DictionaryAdapter(target)),
			_ => null
		};

		if (executeMap is not null)
		{
			executeMap.Invoke();
			return;
		}

		foreach (var sourceField in sourceType.Fields().Values)
		{
			if (sourceField.Attributes.OfType<MapIgnoreAttribute>()
				.Any(_ => _.Type is null || _.Type == target.GetType()))
				continue;

			var sourceValue = sourceField.GetValue(source);
			var mapAttributes = sourceField.Attributes.OfType<MapAttribute>()
				.Where(_ => _.Type == target.GetType());
			if (mapAttributes.Any())
			{
				foreach (var attribute in mapAttributes)
					setFieldValue(attribute.Member, sourceValue);
			}
			else if (sourceField.FieldType.ScalarType() is not ScalarType.None
				|| sourceField.FieldType.IsValueType)
				setFieldValue(sourceField.Name, sourceValue);
		}

		void setFieldValue(string fieldName, object? value)
		{
			var field = target.GetType().Fields()[fieldName];

			if (value is null)
			{
				if (!field.FieldType.IsNullable())
					return;

				field.SetValue(target, null);
				return;
			}

			if (field.FieldType.IsValueType || field.FieldType.ScalarType() is not ScalarType.None)
			{
				field.SetValue(target, value);
				return;
			}

			var targetValue = field.GetValue(target);
			if (targetValue is null)
			{
				targetValue = field.FieldType.Create();
				if (targetValue is null)
					return;

				field.SetValue(target, targetValue);
			}

			this.Map(value, targetValue);
		}
	}

	private void _Map(Array source, Array target)
	{
		if (source.Length is 0 || target.Length is 0)
			return;

		var sourceElementType = source.GetType().GetElementType()!;
		var targetElementType = target.GetType().GetElementType()!;
		if (targetElementType.ScalarType() is not ScalarType.None
			|| sourceElementType.IsValueType)
			Array.Copy(source, 0, target, 0, target.Length);
		else if (!targetElementType.IsValueType)
		{
			var length = source.Length < target.Length ? source.Length : target.Length;
			for (var i = 0; i < length; ++i)
			{
				var sourceValue = source.GetValue(i);
				var targetValue = target!.GetValue(i);
				if (sourceValue is null)
					target.SetValue(null, i);
				else if (targetValue is not null)
					this.Map(sourceValue, targetValue);
			}
		}
	}

	private void _Map(Array source, IList<object> target)
	{
		var sourceItemType = source.GetType().GenericTypeArguments[0];
		var targetItemType = target.GetType().GenericTypeArguments[0];

		if (targetItemType.ScalarType() is not ScalarType.None
			|| sourceItemType.IsValueType)
		{
			target.Clear();
			for (var i = 0; i < source.Length; ++i)
			{
				var value = source.GetValue(i);
				if (value is not null)
					target.Add(value);
			}
		}
		else if (!targetItemType.IsValueType && source.Length > 0)
		{
			var length = source.Length < target.Count ? source.Length : target.Count;
			for (var i = 0; i < length; ++i)
			{
				var sourceValue = source.GetValue(i);
				var targetValue = target[i];
				if (sourceValue is not null && targetValue is not null)
					this.Map(sourceValue, targetValue);
			}
		}
	}

	private void _Map(Array source, IReadOnlyList<object> target)
	{
		var sourceItemType = source.GetType().GenericTypeArguments[0];
		var targetItemType = target.GetType().GenericTypeArguments[0];

		var length = source.Length < target.Count ? source.Length : target.Count;
		if (source.Length > 0
			&& !sourceItemType.IsValueType
			&& !targetItemType.IsValueType
			&& targetItemType.ScalarType() is ScalarType.None)
		{
			for (var i = 0; i < length; ++i)
			{
				var sourceValue = source.GetValue(i);
				var targetValue = target[i];
				if (sourceValue is not null && targetValue is not null)
					this.Map(sourceValue, targetValue);
			}
		}
	}

	private void _Map(Array source, ICollection<object> target)
	{
		var sourceItemType = source.GetType().GenericTypeArguments[0];
		var targetItemType = target.GetType().GenericTypeArguments[0];

		var length = source.Length < target.Count ? source.Length : target.Count;
		if (targetItemType.ScalarType() is not ScalarType.None
			|| sourceItemType.IsValueType)
		{
			target.Clear();
			for (var i = 0; i < source.Length; ++i)
			{
				var value = source.GetValue(i);
				if (value is not null)
					target.Add(value);
			}
		}
	}

	private void _Map(IReadOnlyList<object> source, Array target)
	{
		if (source.Count is 0 || target.Length is 0)
			return;

		var sourceItemType = source.GetType().GenericTypeArguments[0];
		var targetElementType = target.GetType().GetElementType()!;
		if (targetElementType.ScalarType() is not ScalarType.None
			|| sourceItemType.IsValueType)
			source.ForEach((item, index) => target.SetValue(item, index));
		else if (!targetElementType.IsValueType)
		{
			var length = source.Count < target.Length ? source.Count : target.Length;
			for (var i = 0; i < length; ++i)
			{
				var sourceValue = source[i];
				var targetValue = target!.GetValue(i);
				if (sourceValue is null)
					target.SetValue(null, i);
				else if (targetValue is not null)
					this.Map(sourceValue, targetValue);
			}
		}
	}

	private void _Map(IReadOnlyList<object> source, IList<object> target)
	{
		var sourceItemType = source.GetType().GenericTypeArguments[0];
		var targetItemType = target.GetType().GenericTypeArguments[0];
		if (targetItemType.ScalarType() is not ScalarType.None
			|| sourceItemType.IsValueType)
		{
			target.Clear();
			source.WhereNotNull().ForEach(item => target.Add(item));
		}
		else if (!targetItemType.IsValueType && source.Count > 0)
		{
			var length = source.Count < target.Count ? source.Count : target.Count;
			for (var i = 0; i < length; ++i)
			{
				var sourceValue = source[i];
				var targetValue = target[i];
				if (sourceValue is not null && targetValue is not null)
					this.Map(sourceValue, targetValue);
			}
		}
	}

	private void _Map(IReadOnlyList<object> source, IReadOnlyList<object> target)
	{
		var sourceItemType = source.GetType().GenericTypeArguments[0];
		var targetItemType = target.GetType().GenericTypeArguments[0];

		if (source.Count > 0
			&& !sourceItemType.IsValueType
			&& !targetItemType.IsValueType
			&& targetItemType.ScalarType() is ScalarType.None)
		{
			var length = source.Count < target.Count ? source.Count : target.Count;
			for (var i = 0; i < length; ++i)
			{
				var sourceValue = source[i];
				var targetValue = target[i];
				if (sourceValue is not null && targetValue is not null)
					this.Map(sourceValue, targetValue);
			}
		}
	}

	private void _Map(IReadOnlyList<object> source, ICollection<object> target)
	{
		var sourceItemType = source.GetType().GenericTypeArguments[0];
		var targetItemType = target.GetType().GenericTypeArguments[0];

		if (targetItemType.ScalarType() is not ScalarType.None
			|| sourceItemType.IsValueType)
		{
			target.Clear();
			source.WhereNotNull().ForEach(target.Add);
		}
	}

	private void _Map(ICollection<object> source, ICollection<object> target)
	{
		var sourceItemType = source.GetType().GenericTypeArguments[0];
		var targetItemType = target.GetType().GenericTypeArguments[0];

		if (targetItemType.ScalarType() is not ScalarType.None
			|| sourceItemType.IsValueType)
		{
			target.Clear();
			source.WhereNotNull().ForEach(target.Add);
		}
	}

	private void _Map(IDictionary<object, object> source, IDictionary<object, object> target)
	{
		var sourceGenericTypeArguments = source.GetType().GenericTypeArguments;
		var sourceKeyType = sourceGenericTypeArguments[0];
		var sourceValueType = sourceGenericTypeArguments[1];
		var targetGenericTypeArguments = target.GetType().GenericTypeArguments;
		var targetKeyType = targetGenericTypeArguments[0];
		var targetValueType = targetGenericTypeArguments[1];

		if (sourceKeyType.ScalarType() is not ScalarType.None
				&& targetKeyType.ScalarType() is not ScalarType.None
			|| sourceKeyType.IsValueType && targetKeyType.IsValueType)
		{
			if (sourceValueType.ScalarType() is not ScalarType.None
					&& targetValueType.ScalarType() is not ScalarType.None
				|| sourceValueType.IsValueType && targetValueType.IsValueType)
			{
				source.Keys.ForEach(key => target[key] = source[key]);
			}
			else
			{
				source.Keys.Where(target.ContainsKey).ForEach(key => this.Map(source[key], target[key]));
			}
		}
	}

	private void _Map(IReadOnlyDictionary<object, object> source, IDictionary<object, object> target)
	{
		var sourceGenericTypeArguments = source.GetType().GenericTypeArguments;
		var sourceKeyType = sourceGenericTypeArguments[0];
		var sourceValueType = sourceGenericTypeArguments[1];
		var targetGenericTypeArguments = target.GetType().GenericTypeArguments;
		var targetKeyType = targetGenericTypeArguments[0];
		var targetValueType = targetGenericTypeArguments[1];

		if (sourceKeyType.ScalarType() is not ScalarType.None
				&& targetKeyType.ScalarType() is not ScalarType.None
			|| sourceKeyType.IsValueType && targetKeyType.IsValueType)
		{
			if (sourceValueType.ScalarType() is not ScalarType.None
					&& targetValueType.ScalarType() is not ScalarType.None
				|| sourceValueType.IsValueType && targetValueType.IsValueType)
			{
				source.Keys.ForEach(key => target[key] = source[key]);
			}
			else
			{
				source.Keys.Where(target.ContainsKey).ForEach(key => this.Map(source[key], target[key]));
			}
		}
	}
}
