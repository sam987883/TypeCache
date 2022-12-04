// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.GraphQL.Extensions;

public sealed class Accessor<T> : IAccessor<T>
	where T : class, IName
{
	private readonly IReadOnlyDictionary<string, T> _Items;

	public Accessor(IEnumerable<T> items)
	{
		this._Items = items
			.ToDictionary(item => item.Name, item => item)
			.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase);
	}

	public T? this[string name] => this._Items.TryGetValue(name, out var item) ? item : default;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool Has(string name)
		=> this._Items.ContainsKey(name);
}
