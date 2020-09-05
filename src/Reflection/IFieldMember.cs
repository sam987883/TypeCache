// Copyright (c) 2020 Samuel Abraham

using System;

namespace Sam987883.Reflection
{
	public interface IFieldMember<in T> : IMember
		where T : class
	{
		object? this[T instance] { get; set; }

		Delegate? Getter { get; }

		Func<T, object?>? GetValue { get; }

		Delegate? Setter { get; }

		Action<T, object?>? SetValue { get; }
	}
}
