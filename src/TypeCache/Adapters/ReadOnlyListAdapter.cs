// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Adapters;

internal sealed class ReadOnlyListAdapter(object list) : IReadOnlyList<object>
{
	private readonly IReadOnlyDictionary<string, MethodSet> _Methods = list.GetType().Methods();
	private readonly IReadOnlyDictionary<string, PropertyEntity> _Properties = list.GetType().Properties();

	public object this[int index]
	{
		get => this._Properties["Item"][ValueTuple.Create(index)].GetValue(list)!;
	}

	public int Count => (int)this._Properties[nameof(Count)].GetValue(list)!;

	public IEnumerator<object> GetEnumerator()
		=> (IEnumerator<object>)this._Methods[nameof(GetEnumerator)].Invoke(list)!;

	IEnumerator IEnumerable.GetEnumerator()
		=> this.GetEnumerator();
}
