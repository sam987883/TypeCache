// Copyright (c) 2020 Samuel Abraham

using System.Collections.Generic;

namespace sam987883.Reflection.Accessors
{
	public interface IFieldAccessor
	{
		object? this[string key] { get; set; }

		string[] Names { get; }

		IDictionary<string, object?> Values { get; set; }
	}

	public interface IFieldAccessorFactory<T>
		where T : class
	{
		IFieldAccessor Create(T instance);
	}

	public interface IPropertyAccessor
	{
		object? this[string key] { get; set; }

		string[] Names { get; }

		IDictionary<string, object?> Values { get; set; }
	}

	public interface IPropertyAccessorFactory<T>
		where T : class
	{
		IPropertyAccessor Create(T instance);
	}
}
