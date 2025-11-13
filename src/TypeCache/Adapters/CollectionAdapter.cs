// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Adapters;

public class CollectionAdapter : ICollection<object>
{
	protected readonly object _Collection;

	private readonly PropertyEntity _Count;
	private readonly PropertyEntity _IsReadOnly;
	private readonly MethodEntity _Add;
	private readonly MethodEntity _Clear;
	private readonly MethodEntity _Contains;
	private readonly MethodEntity _CopyTo;
	private readonly MethodEntity _Remove;

	public CollectionAdapter(object collection)
	{
		collection.ThrowIfNull();
		(collection.GetType().Implements(typeof(ICollection<>))).ThrowIfFalse();

		this._Collection = collection;

		var collectionType = collection.GetType().GetInterfaces().First(_ => _.Is(typeof(ICollection<>)));

		this._Count = collectionType.Properties[nameof(Count)];
		this._IsReadOnly = collectionType.Properties[nameof(IsReadOnly)];
		this._Add = collectionType.Methods[nameof(Add)].First();
		this._Clear = collectionType.Methods[nameof(Clear)].First()!;
		this._Contains = collectionType.Methods[nameof(Contains)].First();
		this._CopyTo = collectionType.Methods[nameof(CopyTo)].First();
		this._Remove = collectionType.Methods[nameof(Remove)].First();
	}

	public int Count => (int)this._Count.GetValue(this._Collection)!;

	public bool IsReadOnly => (bool)this._IsReadOnly.GetValue(this._Collection)!;

	public void Add(object item)
		=> this._Add.Invoke(this._Collection, [item]);

	public void Clear()
		=> this._Clear.Invoke(this._Collection);

	public bool Contains(object item)
		=> (bool)this._Contains.Invoke(this._Collection, [item])!;

	public void CopyTo(object[] array, int arrayIndex)
		=> this.ForEach(item => array[arrayIndex++] = item);

	public IEnumerator GetEnumerator()
		=> ((IEnumerable)this._Collection).GetEnumerator();

	IEnumerator<object> IEnumerable<object>.GetEnumerator()
		=> ((IEnumerable)this._Collection).OfType<object>().GetEnumerator();

	public virtual bool Remove(object item)
		=> (bool)this._Remove.Invoke(this._Collection, [item])!;
}
