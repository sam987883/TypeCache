// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Adapters;

public class ReadOnlyListAdapter : ReadOnlyCollectionAdapter, IReadOnlyList<object>
{
	private readonly object _List;
	private readonly PropertyIndexerEntity _Item;

	public ReadOnlyListAdapter(object list)
		: base(list)
	{
		this._List = list;
		(list.GetType().Implements(typeof(IReadOnlyList<>))).ThrowIfFalse();

		var listType = list.GetType().GetInterfaces().First(_ => _.Is(typeof(IReadOnlyList<>)));

		this._Item = listType.DefaultIndexer!;
	}

	public object this[int index] => this._Item.GetValue(this._List, [index])!;
}
