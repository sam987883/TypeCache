// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Adapters;

internal class DictionaryAdapter : IDictionary<object, object>
{
	private readonly object _Dictionary;

	private readonly PropertyEntity _Count;
	private readonly PropertyEntity _Item;
	private readonly PropertyEntity _IsReadOnly;
	private readonly PropertyEntity _Keys;
	private readonly PropertyEntity _Values;
	private readonly MethodSet<MethodEntity> _Add;
	private readonly MethodSet<MethodEntity> _Clear;
	private readonly MethodSet<MethodEntity> _ContainsKey;
	private readonly MethodSet<MethodEntity> _CopyTo;
	private readonly MethodSet<MethodEntity> _GetEnumerator;
	private readonly MethodSet<MethodEntity> _Remove;

	public DictionaryAdapter(object dictionary)
	{
		this._Dictionary = dictionary;

		var methods = dictionary.GetType().Methods();
		var properties = dictionary.GetType().Properties();

		this._Count = properties[nameof(Count)];
		this._Item = properties["Item"];
		this._IsReadOnly = properties[nameof(IsReadOnly)];
		this._Keys = properties[nameof(Keys)];
		this._Values = properties[nameof(Values)];
		this._Add = methods[nameof(Add)];
		this._Clear = methods[nameof(Clear)];
		this._ContainsKey = methods[nameof(ContainsKey)];
		this._CopyTo = methods[nameof(CopyTo)];
		this._GetEnumerator = methods[nameof(GetEnumerator)];
		this._Remove = methods[nameof(Remove)];
	}

	public object this[object key]
	{
		get => this._Item[ValueTuple.Create(key)].GetValue(this._Dictionary)!;
		set => this._Item[ValueTuple.Create(key)].SetValue(this._Dictionary, value);
	}

	public ICollection<object> Keys => new CollectionAdapter(this._Keys.GetValue(this._Dictionary)!);

	public ICollection<object> Values => new CollectionAdapter(this._Values.GetValue(this._Dictionary)!);

	public int Count => (int)_Count.GetValue(this._Dictionary)!;

	public bool IsReadOnly => (bool)_IsReadOnly.GetValue(this._Dictionary)!;

	public void Add(object key, object value)
		=> _ = this._Add.Find([key, value])!.Invoke(this._Dictionary, [key, value]);

	public void Add(KeyValuePair<object, object> item)
		=> throw new NotImplementedException();

	public void Clear()
		=> _ = this._Clear.FindWithNoArguments()!.Invoke(this._Dictionary);

	public bool Contains(KeyValuePair<object, object> item)
		=> throw new NotImplementedException();

	public bool ContainsKey(object key)
		=> (bool)this._ContainsKey.Find(key.ToValueTuple())!.Invoke(this._Dictionary, [key])!;

	public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
		=> throw new NotImplementedException();

	public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
		=> throw new NotImplementedException();

	public bool Remove(object key)
		=> (bool)this._Remove.Find(key.ToValueTuple())!.Invoke(this._Dictionary, [key])!;

	public bool Remove(KeyValuePair<object, object> item)
		=> throw new NotImplementedException();

	public bool TryGetValue(object key, [MaybeNullWhen(false)] out object value)
		=> throw new NotImplementedException();

	IEnumerator IEnumerable.GetEnumerator()
		=> (IEnumerator)this._GetEnumerator.FindWithNoArguments()!.Invoke(this._Dictionary)!;
}
