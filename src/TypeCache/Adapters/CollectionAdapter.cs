// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Adapters;

internal sealed class CollectionAdapter(object collection) : ICollection<object>
{
	private readonly IReadOnlyDictionary<string, MethodSet> _Methods = collection.GetType().Methods();
	private readonly IReadOnlyDictionary<string, PropertyEntity> _Properties = collection.GetType().Properties();

	public int Count => (int)this._Properties[nameof(Count)].GetValue(collection)!;

	public bool IsReadOnly => (bool)this._Properties[nameof(IsReadOnly)].GetValue(collection)!;

	public void Add(object item)
		=> this._Methods[nameof(Add)].Invoke(collection, ValueTuple.Create(item));

	public void Clear()
		=> this._Methods[nameof(Clear)].Invoke(collection);

	public bool Contains(object item)
		=> (bool)this._Methods[nameof(Contains)].Invoke(collection, ValueTuple.Create(item))!;

	public void CopyTo(object[] array, int arrayIndex)
		=> this._Methods[nameof(CopyTo)].Invoke(collection, (array, arrayIndex));

	public IEnumerator<object> GetEnumerator()
		=> (IEnumerator<object>)this._Methods[nameof(GetEnumerator)].Invoke(collection)!;

	public bool Remove(object item)
		=> (bool)this._Methods[nameof(Remove)].Invoke(collection, ValueTuple.Create(item))!;

	IEnumerator IEnumerable.GetEnumerator()
		=> this.GetEnumerator();
}
