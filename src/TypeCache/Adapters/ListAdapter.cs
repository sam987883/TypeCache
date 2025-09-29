// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Adapters;

internal sealed class ListAdapter : IList<object>
{
	private readonly object _List;

	private readonly PropertyEntity _Count;
	private readonly PropertyEntity _Item;
	private readonly PropertyEntity _IsReadOnly;
	private readonly MethodSet<MethodEntity> _Add;
	private readonly MethodSet<MethodEntity> _Clear;
	private readonly MethodSet<MethodEntity> _Contains;
	private readonly MethodSet<MethodEntity> _CopyTo;
	private readonly MethodSet<MethodEntity> _GetEnumerator;
	private readonly MethodSet<MethodEntity> _IndexOf;
	private readonly MethodSet<MethodEntity> _Insert;
	private readonly MethodSet<MethodEntity> _Remove;
	private readonly MethodSet<MethodEntity> _RemoveAt;

	public ListAdapter(object list)
	{
		this._List = list;

		var methods = list.GetType().Methods();
		var properties = list.GetType().Properties();

		this._Count = properties[nameof(Count)];
		this._Item = properties["Item"];
		this._IsReadOnly = properties[nameof(IsReadOnly)];
		this._Add = methods[nameof(Add)];
		this._Clear = methods[nameof(Clear)];
		this._Contains = methods[nameof(Contains)];
		this._CopyTo = methods[nameof(CopyTo)];
		this._GetEnumerator = methods[nameof(GetEnumerator)];
		this._IndexOf = methods[nameof(IndexOf)];
		this._Insert = methods[nameof(Insert)];
		this._Remove = methods[nameof(Remove)];
		this._RemoveAt = methods[nameof(RemoveAt)];
	}

	public object this[int index]
	{
		get => this._Item[index.ToValueTuple()].GetValue(this._List)!;
		set => this._Item[index.ToValueTuple()].SetValue(this._List, value);
	}

	public int Count => (int)this._Count.GetValue(this._List)!;

	public bool IsReadOnly => (bool)this._IsReadOnly.GetValue(this._List)!;

	public void Add(object item)
		=> this._Add.Find([item])!.Invoke(this._List, item.ToValueTuple());

	public void Clear()
		=> this._Clear.FindWithNoArguments()!.Invoke(this._List);

	public bool Contains(object item)
		=> (bool)this._Contains.Find([item])!.Invoke(this._List, item.ToValueTuple())!;

	public void CopyTo(object[] array, int arrayIndex)
		=> this._CopyTo.Find([array, arrayIndex])!.Invoke(this._List, (array, arrayIndex));

	public IEnumerator<object> GetEnumerator()
		=> (IEnumerator<object>)this._GetEnumerator.Find([])!.Invoke(this._List)!;

	public int IndexOf(object item)
		=> (int)this._IndexOf.Find([item])!.Invoke(this._List, item.ToValueTuple())!;

	public void Insert(int index, object item)
		=> _ = this._Insert.Find([index, item])!.Invoke(this._List, (index, item))!;

	public bool Remove(object item)
		=> (bool)this._Remove.Find([item])!.Invoke(this._List, item.ToValueTuple())!;

	public void RemoveAt(int index)
		=> _ = this._RemoveAt.Find(index.ToValueTuple())!.Invoke(this._List, index.ToValueTuple())!;

	IEnumerator IEnumerable.GetEnumerator()
		=> this.GetEnumerator();
}
