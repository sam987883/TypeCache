// Copyright (c) 2020 Samuel Abraham

using System.Collections.Generic;

namespace Sam987883.Reflection
{
	public interface IPropertyAccessor
	{
		object? this[string key] { get; set; }

		string[] Names { get; }

		IDictionary<string, object?> Values { get; set; }
	}
}
