// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

[DebuggerDisplay("Property indexer: {name}")]
public readonly struct PropertyIndexer(string name, Lazy<Delegate>? getter, Lazy<Delegate>? setter, ITuple index)
{
	public object? GetStaticValue()
	{
		var getValue = getter?.Value as Func<ITuple, object?>;
		getValue.ThrowIfNull(message: Invariant($"Property [{name}] does not have a static property getter that has arguments."));

		return getValue(index);
	}

	/// <param name="instance">The object instance to get the property value from.</param>
	public object? GetValue(object instance)
	{
		var getValue = getter?.Value as Func<object, ITuple, object?>;
		getValue.ThrowIfNull(message: Invariant($"Property [{name}] does not have a property getter that has arguments."));

		return getValue(instance, index);
	}

	/// <param name="value">The property value to set.</param>
	public void SetStaticValue(object? value)
	{
		var setValue = setter?.Value as Action<ITuple, object?>;
		setValue.ThrowIfNull(message: Invariant($"Property [{name}] does not have a static property setter that has arguments."));

		setValue(index, value);
	}

	/// <param name="instance">The object instance to set the property value on.</param>
	/// <param name="value">The property value to set.</param>
	public void SetValue(object instance, object? value)
	{
		var setValue = setter?.Value as Action<object, ITuple, object?>;
		setValue.ThrowIfNull(message: Invariant($"Property [{name}] does not have a property setter that has arguments."));

		setValue(instance, index, value);
	}

	public bool TryGetStaticValue(out object? value)
	{
		var getValue = getter?.Value as Func<ITuple, object?>;
		var success = getValue is not null;

		value = getValue?.Invoke(index);
		return success;
	}

	public bool TryGetValue(object instance, out object? value)
	{
		var getValue = getter?.Value as Func<object, ITuple, object?>;
		var success = getValue is not null;

		value = getValue?.Invoke(instance, index);
		return success;
	}

	public bool TrySetStaticValue(object? value)
	{
		var setValue = setter?.Value as Action<ITuple, object?>;
		var success = setValue is not null;

		setValue?.Invoke(index, value);
		return success;
	}

	public bool TrySetValue(object instance, object? value)
	{
		var setValue = setter?.Value as Action<object, ITuple, object?>;
		var success = setValue is not null;

		setValue?.Invoke(instance, index, value);
		return success;
	}
}
