// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Adapters;

internal class ReadOnlyDictionaryAdapter : IReadOnlyDictionary<object, object>
{
	private readonly object _Dictionary;

	private readonly PropertyEntity _Count;
	private readonly PropertyEntity _Item;
	private readonly PropertyEntity _Keys;
	private readonly PropertyEntity _Values;
	private readonly MethodSet<MethodEntity> _ContainsKey;
	private readonly MethodSet<MethodEntity> _GetEnumerator;

	public ReadOnlyDictionaryAdapter(object dictionary)
	{
		this._Dictionary = dictionary;

		var methods = dictionary.GetType().Methods();
		var properties = dictionary.GetType().Properties();

		this._Count = properties[nameof(Count)];
		this._Item = properties["Item"];
		this._Keys = properties[nameof(Keys)];
		this._Values = properties[nameof(Values)];
		this._ContainsKey = methods[nameof(ContainsKey)];
		this._GetEnumerator = methods[nameof(GetEnumerator)];
	}

	public object this[object key] => this._Item[ValueTuple.Create(key)].GetValue(this._Dictionary)!;

	public IEnumerable<object> Keys => ((IEnumerable)this._Keys.GetValue(this._Dictionary)!).Cast<object>();

	public IEnumerable<object> Values => ((IEnumerable)this._Values.GetValue(this._Dictionary)!).Cast<object>();

	public int Count => (int)this._Count.GetValue(this._Dictionary)!;

	public bool ContainsKey(object key)
		=> (bool)this._ContainsKey.Find(key.ToValueTuple())!.Invoke(this._Dictionary, [key])!;

	public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
		=> throw new NotImplementedException();

	public bool TryGetValue(object key, [MaybeNullWhen(false)] out object value)
		=> throw new NotImplementedException();

	IEnumerator IEnumerable.GetEnumerator()
		=> (IEnumerator)this._GetEnumerator.FindWithNoArguments()!.Invoke(this._Dictionary)!;
}
