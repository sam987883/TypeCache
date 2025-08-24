// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Adapters;

internal class ReadOnlyDictionaryAdapter(object dictionary) : IReadOnlyDictionary<object, object>
{
	protected readonly IReadOnlyDictionary<string, MethodSet> _Methods = dictionary.GetType().Methods();
	protected readonly IReadOnlyDictionary<string, PropertyEntity> _Properties = dictionary.GetType().Properties();

	public object this[object key] => this._Properties["Item"][ValueTuple.Create(key)].GetValue(dictionary)!;

	public IEnumerable<object> Keys => ((IEnumerable)this._Properties[nameof(Keys)].GetValue(dictionary)!).Cast<object>();

	public IEnumerable<object> Values => ((IEnumerable)this._Properties[nameof(Values)].GetValue(dictionary)!).Cast<object>();

	public int Count => (int)this._Properties[nameof(Count)].GetValue(dictionary)!;

	public bool ContainsKey(object key)
		=> (bool)this._Methods[nameof(ContainsKey)].Invoke(dictionary, [key])!;

	public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
		=> throw new NotImplementedException();

	public bool TryGetValue(object key, [MaybeNullWhen(false)] out object value)
		=> throw new NotImplementedException();

	IEnumerator IEnumerable.GetEnumerator()
		=> (IEnumerator)Type<IEnumerable>.Methods[nameof(GetEnumerator)].Invoke(dictionary)!;
}
