// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Adapters;

internal sealed class CollectionAdapter : ICollection<object>
{
	private readonly object _Collection;

	private readonly PropertyEntity _Count;
	private readonly PropertyEntity _IsReadOnly;
	private readonly MethodSet<MethodEntity> _Add;
	private readonly MethodSet<MethodEntity> _Clear;
	private readonly MethodSet<MethodEntity> _Contains;
	private readonly MethodSet<MethodEntity> _CopyTo;
	private readonly MethodSet<MethodEntity> _GetEnumerator;
	private readonly MethodSet<MethodEntity> _Remove;

	public CollectionAdapter(object collection)
	{
		this._Collection = collection;

		var methods = collection.GetType().Methods();
		var properties = collection.GetType().Properties();

		this._Count = properties[nameof(Count)];
		this._IsReadOnly = properties[nameof(IsReadOnly)];
		this._Add = methods[nameof(Add)];
		this._Clear = methods[nameof(Clear)];
		this._Contains = methods[nameof(Contains)];
		this._CopyTo = methods[nameof(CopyTo)];
		this._GetEnumerator = methods[nameof(GetEnumerator)];
		this._Remove = methods[nameof(Remove)];
	}

	public int Count => (int)this._Count.GetValue(this._Collection)!;

	public bool IsReadOnly => (bool)this._IsReadOnly.GetValue(this._Collection)!;

	public void Add(object item)
		=> this._Add.Find([item])!.Invoke(this._Collection, item.ToValueTuple());

	public void Clear()
		=> this._Clear.FindWithNoArguments()!.Invoke(this._Collection);

	public bool Contains(object item)
		=> (bool)this._Contains.Find([item])!.Invoke(this._Collection, item.ToValueTuple())!;

	public void CopyTo(object[] array, int arrayIndex)
		=> this._CopyTo.Find([array, arrayIndex])!.Invoke(this._Collection, [array, arrayIndex]);

	public IEnumerator<object> GetEnumerator()
		=> (IEnumerator<object>)this._GetEnumerator.FindWithNoArguments()!.Invoke(this._Collection)!;

	public bool Remove(object item)
		=> (bool)this._Remove.Find([item])!.Invoke(this._Collection, item.ToValueTuple())!;

	IEnumerator IEnumerable.GetEnumerator()
		=> this.GetEnumerator();
}
