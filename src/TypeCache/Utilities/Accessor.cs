// Copyright (c) 2021 Samuel Abraham

using System.Collections.Frozen;
using TypeCache.Extensions;

namespace TypeCache.Utilities;

public sealed class Accessor<T>(IEnumerable<T> items)
	: IAccessor<T>
	where T : class, IName
{
	private readonly IReadOnlyDictionary<string, T> _Items = items
		.ToDictionary(item => item.Name, item => item)
		.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

	public T? this[string name] => this._Items.TryGetValue(name, out var item) ? item : default;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool Has(string name)
		=> this._Items.ContainsKey(name);
}
