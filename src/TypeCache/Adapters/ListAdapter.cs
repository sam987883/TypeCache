// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Adapters;

public class ListAdapter : CollectionAdapter, IList<object>
{
	private readonly PropertyIndexerEntity _DefaultIndexer;
	private readonly MethodEntity _IndexOf;
	private readonly MethodEntity _Insert;
	private readonly MethodEntity _RemoveAt;

	public ListAdapter(object list)
		: base(list)
	{
		list.ThrowIfNull();
		(list.GetType().Implements(typeof(IList<>))).ThrowIfFalse();

		var listType = list.GetType().GetInterfaces().First(_ => _.Is(typeof(IList<>)));

		this._DefaultIndexer = listType.DefaultIndexer()!;
		this._IndexOf = listType.Methods()[nameof(IndexOf)].First();
		this._Insert = listType.Methods()[nameof(Insert)].First();
		this._RemoveAt = listType.Methods()[nameof(RemoveAt)].First();
	}

	public object this[int index]
	{
		get => this._DefaultIndexer.GetValue(this._Collection, index.ToValueTuple())!;
		set => this._DefaultIndexer.SetValue(this._Collection, index.ToValueTuple(), value);
	}

	public int IndexOf(object item)
		=> (int)this._IndexOf.Invoke(this._Collection, item.ToValueTuple())!;

	public void Insert(int index, object item)
		=> _ = this._Insert.Invoke(this._Collection, (index, item))!;

	public void RemoveAt(int index)
		=> _ = this._RemoveAt.Invoke(this._Collection, index.ToValueTuple())!;
}
