// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.Reflection
{
	public interface IFieldMember : IMember
	{
		object? this[object instance] { get; set; }

		Delegate? Getter { get; }

		Func<object, object?>? GetValue { get; }

		Delegate? Setter { get; }

		Action<object, object?>? SetValue { get; }
	}
}
