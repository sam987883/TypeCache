// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Adapters;

internal sealed class ListAdapter(object list) : IList<object>
{
	private readonly IReadOnlyDictionary<string, MethodSet> _Methods = list.GetType().Methods();
	private readonly IReadOnlyDictionary<string, PropertyEntity> _Properties = list.GetType().Properties();

	public object this[int index]
	{
		get => this._Properties["Item"][ValueTuple.Create(index)].GetValue(list)!;
		set => this._Properties["Item"][ValueTuple.Create(index)].SetValue(list, value);
	}

	public int Count => (int)this._Properties[nameof(Count)].GetValue(list)!;

	public bool IsReadOnly => (bool)this._Properties[nameof(IsReadOnly)].GetValue(list)!;

	public void Add(object item)
		=> this._Methods[nameof(Add)].Invoke(list, ValueTuple.Create(item));

	public void Clear()
		=> this._Methods[nameof(Clear)].Invoke(list);

	public bool Contains(object item)
		=> (bool)this._Methods[nameof(Contains)].Invoke(list, ValueTuple.Create(item))!;

	public void CopyTo(object[] array, int arrayIndex)
		=> this._Methods[nameof(CopyTo)].Invoke(list, (array, arrayIndex));

	public IEnumerator<object> GetEnumerator()
		=> (IEnumerator<object>)this._Methods[nameof(GetEnumerator)].Invoke(list)!;

	public int IndexOf(object item)
		=> (int)this._Methods[nameof(IndexOf)].Invoke(list, ValueTuple.Create(item))!;

	public void Insert(int index, object item)
		=> _ = this._Methods[nameof(Insert)].Invoke(list, (index, item))!;

	public bool Remove(object item)
		=> (bool)this._Methods[nameof(Remove)].Invoke(list, ValueTuple.Create(item))!;

	public void RemoveAt(int index)
		=> _ = this._Methods[nameof(RemoveAt)].Invoke(list, [index])!;

	IEnumerator IEnumerable.GetEnumerator()
		=> this.GetEnumerator();
}
