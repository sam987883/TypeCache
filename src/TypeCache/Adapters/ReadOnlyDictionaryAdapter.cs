// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Adapters;

public class ReadOnlyDictionaryAdapter : ReadOnlyCollectionAdapter, IReadOnlyDictionary<object, object>
{
	private readonly object _Dictionary;

	private readonly PropertyIndexerEntity _Item;
	private readonly PropertyEntity _Keys;
	private readonly PropertyEntity _Values;
	private readonly MethodSet<MethodEntity> _ContainsKey;

	public ReadOnlyDictionaryAdapter(object dictionary)
		: base(dictionary)
	{
		this._Dictionary = dictionary;
		(dictionary.GetType().Implements(typeof(IReadOnlyDictionary<,>))).ThrowIfFalse();

		var dictionaryType = dictionary.GetType().GetInterfaces().First(_ => _.Is(typeof(IReadOnlyDictionary<,>)));

		this._Item = dictionaryType.DefaultIndexer!;
		this._Keys = dictionaryType.Properties[nameof(Keys)];
		this._Values = dictionaryType.Properties[nameof(Values)];
		this._ContainsKey = dictionaryType.Methods[nameof(ContainsKey)];
	}

	public object this[object key] => this._Item.GetValue(this._Dictionary, [key])!;

	public IEnumerable<object> Keys => ((IEnumerable)this._Keys.GetValue(this._Dictionary)!).Cast<object>();

	public IEnumerable<object> Values => ((IEnumerable)this._Values.GetValue(this._Dictionary)!).Cast<object>();

	public bool ContainsKey(object key)
		=> (bool)this._ContainsKey.Find(key.ToValueTuple())!.Invoke(this._Dictionary, [key])!;

	IEnumerator<KeyValuePair<object, object>> IEnumerable<KeyValuePair<object, object>>.GetEnumerator()
		=> throw new NotImplementedException();

	public bool TryGetValue(object key, [MaybeNullWhen(false)] out object value)
		=> throw new NotImplementedException();
}
