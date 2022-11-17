// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using static TypeCache.Default;

namespace TypeCache.GraphQL.Extensions;

public sealed class Accessor<T> : IAccessor<T>
	where T : class, IName
{
	private readonly IReadOnlyDictionary<string, T> _Items;

	public Accessor(IEnumerable<T> items)
	{
		this._Items = items
			.ToDictionary(item => item.Name, item => item)
			.ToImmutableDictionary();
	}

	public T? this[string name] => this._Items.TryGetValue(name, out var item) ? item : default;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Has(string name)
		=> this._Items.ContainsKey(name);
}
