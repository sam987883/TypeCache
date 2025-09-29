// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Adapters;

internal sealed class ReadOnlyListAdapter : IReadOnlyList<object>
{
	private readonly object _List;

	private readonly PropertyEntity _Count;
	private readonly PropertyEntity _Item;
	private readonly MethodSet<MethodEntity> _GetEnumerator;

	public ReadOnlyListAdapter(object list)
	{
		this._List = list;

		var methods = list.GetType().Methods();
		var properties = list.GetType().Properties();

		this._Count = properties[nameof(Count)];
		this._Item = properties["Item"];
		this._GetEnumerator = methods[nameof(GetEnumerator)];
	}

	public object this[int index] => this._Item[index.ToValueTuple()].GetValue(this._List)!;

	public int Count => (int)this._Count.GetValue(this._List)!;

	public IEnumerator<object> GetEnumerator()
		=> (IEnumerator<object>)this._GetEnumerator.FindWithNoArguments()!.Invoke(this._List)!;

	IEnumerator IEnumerable.GetEnumerator()
		=> this.GetEnumerator();
}
