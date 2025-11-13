// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Adapters;

public class ReadOnlyCollectionAdapter : IReadOnlyCollection<object>
{
	private readonly object _Collection;
	private readonly PropertyEntity _Count;

	public ReadOnlyCollectionAdapter(object collection)
	{
		this._Collection = collection;
		(collection.GetType().Implements(typeof(IReadOnlyCollection<>))).ThrowIfFalse();

		var collectionType = collection.GetType().GetInterfaces().First(_ => _.Is(typeof(IReadOnlyCollection<>)));

		this._Count = collectionType.Properties[nameof(Count)];
	}

	public int Count => (int)this._Count.GetValue(this._Collection)!;

	public IEnumerator GetEnumerator()
		=> ((IEnumerable)this._Collection).GetEnumerator();

	IEnumerator<object> IEnumerable<object>.GetEnumerator()
		=> ((IEnumerable)this._Collection).OfType<object>().GetEnumerator();
}
