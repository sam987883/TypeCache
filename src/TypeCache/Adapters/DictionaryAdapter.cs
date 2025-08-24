// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Adapters;

internal class DictionaryAdapter(object dictionary) : IDictionary<object, object>
{
	protected readonly IReadOnlyDictionary<string, MethodSet> _Methods = dictionary.GetType().Methods();
	protected readonly IReadOnlyDictionary<string, PropertyEntity> _Properties = dictionary.GetType().Properties();

	public object this[object key]
	{
		get => this._Properties["Item"][ValueTuple.Create(key)].GetValue(dictionary)!;
		set => this._Properties["Item"][ValueTuple.Create(key)].SetValue(dictionary, value);
	}

	public ICollection<object> Keys => new CollectionAdapter(this._Properties[nameof(Keys)].GetValue(dictionary)!);

	public ICollection<object> Values => new CollectionAdapter(this._Properties[nameof(Values)].GetValue(dictionary)!);

	public int Count => (int)this._Properties[nameof(Count)].GetValue(dictionary)!;

	public bool IsReadOnly => (bool)this._Properties[nameof(IsReadOnly)].GetValue(dictionary)!;

	public void Add(object key, object value)
		=> _ = this._Methods[nameof(Add)].Invoke(dictionary, [key, value]);

	public void Add(KeyValuePair<object, object> item)
		=> throw new NotImplementedException();

	public void Clear()
		=> _ = this._Methods[nameof(Clear)].Invoke(dictionary);

	public bool Contains(KeyValuePair<object, object> item)
		=> throw new NotImplementedException();

	public bool ContainsKey(object key)
		=> (bool)this._Methods[nameof(ContainsKey)].Invoke(dictionary, [key])!;

	public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
		=> throw new NotImplementedException();

	public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
		=> throw new NotImplementedException();

	public bool Remove(object key)
		=> (bool)this._Methods[nameof(Remove)].Invoke(dictionary, [key])!;

	public bool Remove(KeyValuePair<object, object> item)
		=> throw new NotImplementedException();

	public bool TryGetValue(object key, [MaybeNullWhen(false)] out object value)
		=> throw new NotImplementedException();

	IEnumerator IEnumerable.GetEnumerator()
		=> (IEnumerator)Type<IEnumerable>.Methods[nameof(GetEnumerator)].Invoke(dictionary)!;
}
