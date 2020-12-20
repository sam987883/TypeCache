// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.Reflection
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
