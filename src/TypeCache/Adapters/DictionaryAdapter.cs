// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Collections.Generic;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Adapters;

public class DictionaryAdapter : CollectionAdapter, IDictionary<object, object>
{
	private readonly object _Dictionary;

	private readonly PropertyIndexerEntity _Item;
	private readonly PropertyEntity _Keys;
	private readonly PropertyEntity _Values;
	private readonly MethodEntity _Add;
	private readonly MethodEntity _ContainsKey;
	private readonly MethodEntity _Remove;

	public DictionaryAdapter(object dictionary)
		: base(dictionary)
	{
		this._Dictionary = dictionary;
		(dictionary.GetType().Implements(typeof(IDictionary<,>))).ThrowIfFalse();

		var dictionaryType = dictionary.GetType().GetInterfaces().First(_ => _.Is(typeof(IDictionary<,>)));

		this._Item = dictionaryType.DefaultIndexer!;
		this._Keys = dictionaryType.Properties[nameof(Keys)];
		this._Values = dictionaryType.Properties[nameof(Values)];
		this._Add = dictionaryType.Methods[nameof(Add)].First(_ => _.Parameters.Count is 2);
		this._ContainsKey = dictionaryType.Methods[nameof(ContainsKey)].First();
		this._Remove = dictionaryType.Methods[nameof(Remove)].First(_ => _.Parameters[0].ParameterType == dictionaryType.GetGenericArguments()[0]);
	}

	public object this[object key]
	{
		get => this._Item.GetValue(this._Dictionary, [key])!;
		set => this._Item.SetValue(this._Dictionary, [key], value);
	}

	public ICollection<object> Keys => new CollectionAdapter(this._Keys.GetValue(this._Dictionary)!);

	public ICollection<object> Values => new CollectionAdapter(this._Values.GetValue(this._Dictionary)!);

	public void Add(object key, object value)
		=> _ = this._Add.Invoke(this._Dictionary, [key, value]);

	public void Add(KeyValuePair<object, object> item)
		=> throw new NotImplementedException();

	public bool Contains(KeyValuePair<object, object> item)
		=> throw new NotImplementedException();

	public bool ContainsKey(object key)
		=> (bool)this._ContainsKey.Invoke(this._Dictionary, [key])!;

	public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
		=> throw new NotImplementedException();

	IEnumerator<KeyValuePair<object, object>> IEnumerable<KeyValuePair<object, object>>.GetEnumerator()
		=> throw new NotImplementedException();

	public override bool Remove(object key)
		=> (bool)this._Remove.Invoke(this._Dictionary, [key])!;

	public bool Remove(KeyValuePair<object, object> item)
		=> throw new NotImplementedException();

	public bool TryGetValue(object key, [MaybeNullWhen(false)] out object value)
		=> throw new NotImplementedException();
}
