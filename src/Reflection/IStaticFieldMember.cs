// Copyright (c) 2020 Samuel Abraham

using System;

namespace Sam987883.Reflection
{
	public interface IStaticFieldMember : IMember
	{
		Delegate? Getter { get; }

		Func<object?>? GetValue { get; }

		Delegate? Setter { get; }

		Action<object?>? SetValue { get; }

		object? Value { get; set; }
	}
}
